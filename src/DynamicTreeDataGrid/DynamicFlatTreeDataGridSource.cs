using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel, TModelKey> : NotifyingBase, IDynamicTreeDataGridSource<TModel>,
    IDisposable
    where TModel : class where TModelKey : notnull {
    private readonly CompositeDisposable _d = new();

    // By default, the filtering function just includes all rows.
    private readonly ISubject<Func<TModel, bool>> _filterSource = new BehaviorSubject<Func<TModel, bool>>(_ => true);
    private readonly ReadOnlyObservableCollection<TModel> _items;
    private readonly IObservable<Func<TModel, bool>> _itemsFilter;
    private readonly IObservable<IComparer<TModel>> _sort;
    private readonly Subject<IComparer<TModel>?> _sortSource = new();

    public DynamicFlatTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes,
                                         IScheduler mainThreadScheduler) {
        _itemsFilter = _filterSource;
        TotalCount = changes.Count();

        // Setup Sort notifications
        _sort = _sortSource.Select(comparer => comparer ?? new NoSortComparer<TModel>());
        var sortDisposable = _sort.Subscribe(comparer => {
            // Reverse the NoSortComparer for this field from FlatTreeDataGridSource
            _comparer = comparer is NoSortComparer<TModel> ? null : comparer;
            Sorted?.Invoke();
        });

        var filteredChanges = changes.Filter(_itemsFilter);
        FilteredCount = filteredChanges.Count();

        var disposable = filteredChanges.DeferUntilLoaded()
            .Sort(_sort) // Use SortAndBind?
            .ObserveOn(mainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe();

        _itemsView = TreeDataGridItemsSourceView<TModel>.GetOrCreate(_items);

        // Setup Disposables
        _d.Add(disposable);
        _d.Add(sortDisposable);

        // TODO: Fix CreateRows()? Does this now work since we set the comparer?
    }

    public DynamicColumnList<TModel> Columns { get; } = [];

    public IObservable<int> FilteredCount { get; }
    public IObservable<int> TotalCount { get; }
    IDynamicColumns IDynamicTreeDataGridSource.Columns => Columns;
    IColumns ITreeDataGridSource.Columns => Columns.DisplayedColumns;

    /// <summary>
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    /// <remarks>Slight changes to <see cref="ITreeDataGridSource.SortBy" /> but DynamicData-aware</remarks>
    public bool SortBy(IColumn? column, ListSortDirection direction) {
        if (column is IColumn<TModel> typedColumn) {
            if (!Columns.Contains(typedColumn))
                return true;

            var comparer = typedColumn.GetComparison(direction);

            if (comparer is not null) {
                var comparerInstance = new FuncComparer<TModel>(comparer);

                // Trigger a new sort notification.
                _sortSource.OnNext(comparerInstance);
                foreach (var c in Columns)
                    c.SortDirection = c == column ? direction : null;
            }

            return true;
        }

        return false;
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
