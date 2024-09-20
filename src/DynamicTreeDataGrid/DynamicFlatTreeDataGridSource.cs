using System.ComponentModel;
using System.Reactive.Subjects;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;

using DynamicData;
using DynamicData.Aggregation;

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel, TModelKey> : IDynamicTreeDataGridSource<TModel>
    where TModel : class where TModelKey : notnull {
    private readonly FlatTreeDataGridSource<TModel> treeDataGridSourceImplementation;
    private readonly IObservable<IChangeSet<TModel, TModelKey>> _changeSet;
    private readonly IObservable<IComparer<TModel>> _sort;
    private readonly ISubject<IComparer<TModel>> _sortSource = new Subject<IComparer<TModel>>();

    private readonly ISubject<Func<TModel, bool>> _filterSource = new BehaviorSubject<Func<TModel, bool>>(model => true);
    private readonly IObservable<Func<TModel, bool>> _itemsFilter;

    public DynamicFlatTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes) {
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

        treeDataGridSourceImplementation = new FlatTreeDataGridSource<TModel>(list);

        // TODO: Setup Sorted event for treeDataGridSourceImplementation?
    }

    public IObservable<int> FilteredCount { get; set; }

    public IObservable<int> TotalCount { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged {
        add => treeDataGridSourceImplementation.PropertyChanged += value;
        remove => treeDataGridSourceImplementation.PropertyChanged -= value;
    }

    public void DragDropRows(ITreeDataGridSource source,
                             IEnumerable<IndexPath> indexes,
                             IndexPath targetIndex,
                             TreeDataGridRowDropPosition position,
                             DragDropEffects effects) {
        ((ITreeDataGridSource)treeDataGridSourceImplementation).DragDropRows(source, indexes, targetIndex, position,
            effects);
    }

    public IEnumerable<object>? GetModelChildren(object model) =>
        ((ITreeDataGridSource)treeDataGridSourceImplementation).GetModelChildren(model);

    public bool SortBy(IColumn? column, ListSortDirection direction) {
        if (column is IColumn<TModel> typedColumn) {
            if (!Columns.Contains(typedColumn))
                return true;

            var comparer = typedColumn.GetComparison(direction);

            if (comparer is not null) {
                var comparerInstance = new FuncComparer<TModel>(comparer);

                // Trigger a new sort notification.
                _sortSource.OnNext(comparerInstance);
                Sorted?.Invoke();
                foreach (var c in Columns)
                    c.SortDirection = c == column ? direction : null;
            }

            return true;
        }

        return false;
    }

    public ColumnList<TModel> Columns => treeDataGridSourceImplementation.Columns;
    IColumns ITreeDataGridSource.Columns => Columns;

    public IRows Rows => treeDataGridSourceImplementation.Rows;

    public ITreeDataGridSelection? Selection => treeDataGridSourceImplementation.Selection;

    public bool IsHierarchical => treeDataGridSourceImplementation.IsHierarchical;

    public bool IsSorted => treeDataGridSourceImplementation.IsSorted;

    IEnumerable<TModel> ITreeDataGridSource<TModel>.Items => treeDataGridSourceImplementation.Items;

    IEnumerable<object> ITreeDataGridSource.Items => ((ITreeDataGridSource)treeDataGridSourceImplementation).Items;

    public event Action? Sorted;
}
