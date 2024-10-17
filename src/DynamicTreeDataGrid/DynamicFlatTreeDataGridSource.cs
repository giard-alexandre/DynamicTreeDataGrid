using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Avalonia.Controls;
using Avalonia.Controls.Models;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;

using DynamicData;
using DynamicData.Aggregation;

using DynamicTreeDataGrid.Models;
using DynamicTreeDataGrid.Models.Columns;
using DynamicTreeDataGrid.Models.Sorting;
using DynamicTreeDataGrid.Models.State;

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel, TModelKey> : NotifyingBase, IDynamicTreeDataGridSource<TModel>,
    IDisposable
    where TModel : class where TModelKey : notnull {
    private readonly CompositeDisposable _d = new();

    // By default, the filtering function just includes all rows.
    private readonly BehaviorSubject<Func<TModel, bool>> _filterSource = new(_ => true);
    private readonly ReadOnlyObservableCollection<TModel> _items;
    private readonly IObservable<Func<TModel, bool>> _itemsFilter;
    private readonly IObservable<IComparer<TModel>> _sort;
    private readonly BehaviorSubject<IComparer<TModel>?> _columnsSortSource = new(null);

    public DynamicFlatTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes,
                                         IScheduler mainThreadScheduler) : this(changes, mainThreadScheduler,
        new DynamicTreeDataGridSourceOptions<TModel>()) { }

    public DynamicFlatTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes,
                                         IScheduler mainThreadScheduler,
                                         DynamicTreeDataGridSourceOptions<TModel> options) {
        Options = options;
        _itemsFilter = _filterSource;
        TotalCount = changes.Count();

        // Setup Sort notifications
        _sort = _columnsSortSource

            // Trigger re-sorting if either column sort changes or the resorter from Options fires.
            .CombineLatest(Options.Resorter.StartWith(Unit.Default), resultSelector: (comparer, _) => comparer)
            .Do(comparer => _comparer = comparer) // TODO: should the comparer be the CombinedComparer?
            .Select(comparer => new CombinedComparer<TModel>(Options.PreColumnSort, comparer, Options.PostColumnSort));
        var sortDisposable = _sort.Subscribe(comparer => { Sorted?.Invoke(); });

        var filteredChanges = changes.Filter(_itemsFilter);
        FilteredCount = filteredChanges.Count();

        var disposable = filteredChanges.DeferUntilLoaded()
            .Sort(_sort, sortOptimisations: Options.SortOptimisations) // Use SortAndBind?
            .ObserveOn(mainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe();

        _itemsView = TreeDataGridItemsSourceView<TModel>.GetOrCreate(_items);

        // Setup Disposables
        _d.Add(disposable);
        _d.Add(sortDisposable);
    }

    public DynamicColumnList<TModel> Columns { get; } = [];

    public IObservable<int> FilteredCount { get; }
    public IObservable<int> TotalCount { get; }
    IDynamicColumns IDynamicTreeDataGridSource.Columns => Columns;
    IColumns ITreeDataGridSource.Columns => Columns.DisplayedColumns;
    public DynamicTreeDataGridSourceOptions<TModel> Options { get; }

    public GridState GetGridState() => new() { ColumnStates = Columns.GetColumnStates() };

    public bool ApplyGridState(GridState state) {
        try {
            bool columnsApplied = Columns.ApplyColumnStates(state.ColumnStates);
            if (!columnsApplied) {
                Console.WriteLine("Error applying column state");
                return false;
            }

            // We keep only the first column to sort by here and ignore any extras
            ColumnState? sortedColumnState = null;
            int sortedColumns = 0;
            foreach (var colState in state.ColumnStates)
            {
                if (colState.SortDirection is not null)
                {
                    sortedColumns++;
                    sortedColumnState ??= colState;
                }
            }

            if (sortedColumns > 1)
            {
                Console.WriteLine($"""
                                   {nameof(DynamicFlatTreeDataGridSource<TModel, TModelKey>)}: More than one sorted
                                   column was provided to the `ApplyGridState` method. This isn't an error but
                                   only the first one was applied to the grid.
                                   """);
            }

            // Trigger column sorting if we found a column to sort by.
            // We only keep the last sorted column in the list on purpose, as we don't support sorting
            // by more than one element atm.
            if (sortedColumnState is not null) {
                SortBy(Columns[sortedColumnState.Index], (ListSortDirection)sortedColumnState.SortDirection!);
            }

            // Set sort comparer once columns have been applied
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("Error applying grid state");
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    /// <remarks>Slight changes to <see cref="ITreeDataGridSource.SortBy" /> but DynamicData-aware</remarks>
    public bool SortBy(IColumn? column, ListSortDirection direction) {
        if (column is not IColumn<TModel> typedColumn) return false;

        if (!Columns.Contains(typedColumn))
            return true;

        var comparer = typedColumn.GetComparison(direction);

        if (comparer is not null) {
            var comparerInstance = new FuncComparer<TModel>(comparer);

            // Trigger a new sort notification.
            _columnsSortSource.OnNext(comparerInstance);
            foreach (var c in Columns)
                c.SortDirection = c == column ? direction : null;
        }

        return true;

    }


    #region From FlatTreeDataGrid

    // private IEnumerable<TModel> _items;
    private readonly TreeDataGridItemsSourceView<TModel> _itemsView;
    private AnonymousSortableRows<TModel>? _rows;
    private IComparer<TModel>? _comparer;
    private ITreeDataGridSelection? _selection;
    private bool _isSelectionSet;


    public IRows Rows => _rows ??= CreateRows();

    public IEnumerable<TModel> Items => _items;

    public ITreeDataGridSelection? Selection {
        get {
            if (_selection == null && !_isSelectionSet)
                _selection = new TreeDataGridRowSelectionModel<TModel>(this);
            return _selection;
        }
        set {
            if (_selection != value) {
                if (value?.Source != _items)
                    throw new InvalidOperationException("Selection source must be set to Items.");
                _selection = value;
                _isSelectionSet = true;
                RaisePropertyChanged();
            }
        }
    }

    IEnumerable<object> ITreeDataGridSource.Items => Items;

    public ITreeDataGridCellSelectionModel<TModel>? CellSelection =>
        Selection as ITreeDataGridCellSelectionModel<TModel>;

    public ITreeDataGridRowSelectionModel<TModel>? RowSelection => Selection as ITreeDataGridRowSelectionModel<TModel>;
    public bool IsHierarchical => false;
    public bool IsSorted => _comparer is not null;

    public event Action? Sorted;

    void ITreeDataGridSource.DragDropRows(ITreeDataGridSource source,
                                          IEnumerable<IndexPath> indexes,
                                          IndexPath targetIndex,
                                          TreeDataGridRowDropPosition position,
                                          DragDropEffects effects) {
        if (effects != DragDropEffects.Move)
            throw new NotSupportedException("Only move is currently supported for drag/drop.");
        if (IsSorted)
            throw new NotSupportedException("Drag/drop is not supported on sorted data.");
        if (position == TreeDataGridRowDropPosition.Inside)
            throw new ArgumentException("Invalid drop position.", nameof(position));
        if (indexes.Any(x => x.Count != 1))
            throw new ArgumentException("Invalid source index.", nameof(indexes));
        if (targetIndex.Count != 1)
            throw new ArgumentException("Invalid target index.", nameof(targetIndex));
        if (_items is not IList<TModel> items)
            throw new InvalidOperationException("Items does not implement IList<T>.");

        if (position == TreeDataGridRowDropPosition.None)
            return;

        var ti = targetIndex[0];

        if (position == TreeDataGridRowDropPosition.After)
            ++ti;

        var sourceItems = new List<TModel>();

        foreach (var src in indexes.OrderByDescending(x => x)) {
            var i = src[0];
            sourceItems.Add(items[i]);
            items.RemoveAt(i);

            if (i < ti)
                --ti;
        }

        for (var si = sourceItems.Count - 1; si >= 0; --si) items.Insert(ti++, sourceItems[si]);
    }

    IEnumerable<object> ITreeDataGridSource.GetModelChildren(object model) => Enumerable.Empty<object>();

    private AnonymousSortableRows<TModel> CreateRows() => new(_itemsView, _comparer);

    #endregion


    #region IDisposable

    private bool _disposed;

    private void Dispose(bool disposing) {
        if (_disposed || !disposing) return;

        _d.Dispose();
        _rows?.Dispose();
        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
