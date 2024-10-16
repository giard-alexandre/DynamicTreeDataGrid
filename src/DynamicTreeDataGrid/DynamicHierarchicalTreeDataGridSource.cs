using System.ComponentModel;
using System.Reactive.Subjects;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicData;
using DynamicData.Aggregation;

using DynamicTreeDataGrid.Models;
using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid;

// TODO: Implement this for real :)
public class DynamicHierarchicalTreeDataGridSource<TModel, TModelKey> : HierarchicalTreeDataGridSource<TModel>,
    IDynamicTreeDataGridSource<TModel>
    where TModel : class where TModelKey : notnull {
    private readonly IObservable<IChangeSet<TModel, TModelKey>> _changeSet;

    // By default, the filtering function just includes all rows.
    private readonly ISubject<Func<TModel, bool>> _filterSource = new BehaviorSubject<Func<TModel, bool>>(_ => true);
    private readonly IObservable<Func<TModel, bool>> _itemsFilter;
    private readonly IObservable<IComparer<TModel>> _sort;
    private readonly ISubject<IComparer<TModel>> _sortSource = new Subject<IComparer<TModel>>();

    public DynamicHierarchicalTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes) : base([]) {
        _itemsFilter = _filterSource;

        // Use RefCount to avoid duplicate work
        _changeSet = changes.RefCount();
        _sort = _sortSource;
        TotalCount = _changeSet.Count();

        var filteredChanges = _changeSet.Filter(_itemsFilter);
        FilteredCount = filteredChanges.Count();

        var myOperation = filteredChanges.Sort(_sort)
            .Bind(out var list)
            .DisposeMany()
            .Subscribe(set => Console.WriteLine("Changeset changed."));

        Items = list;

        // TODO: Setup Sorted event for treeDataGridSourceImplementation?
    }

    public new DynamicColumnList<TModel> Columns { get; } = [];

    public IObservable<int> FilteredCount { get; }
    public IObservable<int> TotalCount { get; }
    IDynamicColumns IDynamicTreeDataGridSource.Columns => Columns;
    IColumns ITreeDataGridSource.Columns => Columns.DisplayedColumns;

    #region Override base sorted logic with IChangeSet sorting

    // TODO: Change to check the sort observable.
    // public bool IsSorted => _comparer is not null;

    public new event Action? Sorted;
    bool ITreeDataGridSource.SortBy(IColumn? column, ListSortDirection direction) => SortBy(column, direction);

    private new bool SortBy(IColumn? column, ListSortDirection direction) {
        // if (column is IColumn<TModel> typedColumn) {
        //     if (!Columns.Contains(typedColumn))
        //         return true;
        //
        //     var comparer = typedColumn.GetComparison(direction);
        //
        //     if (comparer is not null) {
        //         var comparerInstance = new FuncComparer<TModel>(comparer);
        //
        //         // Trigger a new sort notification.
        //         _sortSource.OnNext(comparerInstance);
        //         Sorted?.Invoke();
        //         foreach (var c in Columns)
        //             c.SortDirection = c == column ? direction : null;
        //     }
        //
        //     return true;
        // }
        //
        // return false;
        // TODO: make this use the _sortSource.OnNext method
        if (column is IColumn<TModel> columnBase && Columns.Contains(columnBase) &&
            columnBase.GetComparison(direction) is Comparison<TModel> comparison) {
            Sort(comparison);
            Sorted?.Invoke();
            foreach (var c in Columns)
                c.SortDirection = c == column ? direction : null;
            return true;
        }

        return false;
    }

    #endregion
}
