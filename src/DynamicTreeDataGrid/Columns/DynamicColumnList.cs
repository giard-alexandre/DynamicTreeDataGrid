using System.Collections.Specialized;
using System.ComponentModel;

using DynamicTreeDataGrid.Models.Columns;
using DynamicTreeDataGrid.State;

namespace DynamicTreeDataGrid.Columns;

/// <summary>
///     Maintains 2 lists of columns. The main one, which is exposed by default, is the list of all columns
///     defined. The second, exposed through "DisplayedColumns" is the list that is shown to the user (Visible).
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class DynamicColumnList<TModel> : DynamicColumnListBase<TModel>, IDynamicColumns
    where TModel : class {
    private readonly DynamicColumnListBase<TModel> _displayedColumns = [];

    public DynamicColumnList() {
        CollectionChanged += SyncFilteredCollection;

        // Subscribe to property changes for initial items
        foreach (var item in this) ItemAdded(item);
    }

    /// <summary>
    ///     Columns that are used by the TreeDataGrid. Filters out the columns that should not be visible.
    /// </summary>
    public IDynamicColumnsBase DisplayedColumns => _displayedColumns;

    public void Move(IDynamicColumn oldLocation, IDynamicColumn newLocation) {
        var oldIndex = IndexOf((IDynamicColumn<TModel>)oldLocation);
        var newIndex = IndexOf((IDynamicColumn<TModel>)newLocation);
        Move(oldIndex, newIndex);
    }

    public void Move(int oldIndex, int newIndex) {
        var item = base[oldIndex];
        base.RemoveItem(oldIndex);
        base.InsertItem(newIndex, item);
    }

    public IList<ColumnState> GetColumnStates() {
        IList<ColumnState> states = [];
        for (var i = 0; i < Count; i++) {
            var column = this[i];
            states.Add(new ColumnState(column.Name) { Visible = column.Visible, Index = i, SortDirection = column.SortDirection });
        }

        return states;
    }

    public bool ApplyColumnStates(IEnumerable<ColumnState> states) {
        try {
            var intersections = Items.Join(states, dc => dc.Name, cs => cs.Name,
                    (dynamicColumn, state) => (column: dynamicColumn, state))
                .ToList();

            foreach (var (column, state) in intersections) {
                var oldIndex = IndexOf(column);
                Move(oldIndex, state.Index);
                column.Visible = state.Visible;
                column.SortDirection = state.SortDirection;
            }

            return true;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }

    private void SyncFilteredCollection(object? sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                foreach (var newItem in e.NewItems) ItemAdded(newItem);

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var oldItem in e.OldItems) ItemRemoved(oldItem);

                break;

            case NotifyCollectionChangedAction.Replace:
                ItemReplaced(e);

                break;

            case NotifyCollectionChangedAction.Move:
                // Handle move if needed
                break;

            case NotifyCollectionChangedAction.Reset:
                // Just clear the Displayed ones, we're re-adding everything anyways.
                _displayedColumns.Clear();
                foreach (var column in this) {
                    // Remove it just in case it was already present.
                    column.PropertyChanged -= Item_PropertyChanged;
                    ItemAdded(column);
                }

                break;
        }
    }

    private void ItemAdded(object? item) {
        if (item is not IDynamicColumn<TModel> column) return;
        ItemAdded(column);
    }

    private void ItemAdded(IDynamicColumn<TModel> column) {
        column.PropertyChanged += Item_PropertyChanged;
        if (!column.Visible) return;

        var index = IndexOf(column);
        var offset = GetDisplayedOffset(index);
        _displayedColumns.Insert(index - offset, column);
    }

    private void ItemRemoved(object? item) {
        if (item is not IDynamicColumn<TModel> column) return;
        column.PropertyChanged -= Item_PropertyChanged;
        _displayedColumns.Remove(column);
    }

    private void ItemReplaced(NotifyCollectionChangedEventArgs e) {
        throw new NotImplementedException("Replace is not implemented yet");

        // for (int i = 0; i < e.OldItems.Count; i++) {
        //     _displayedColumns[e.OldStartingIndex + i] = (IDynamicColumn<TModel>)e.NewItems[i];
        // }
        // Unsubscribe from old item changes
        // Subscribe to new item changes
        // Replace in the filtered list too (somehow)
    }

    private int GetDisplayedOffset(int desiredIndex) {
        var indexOffset = 0;
        for (var i = 0; i < desiredIndex; i++)
            if (!this[i].Visible)
                indexOffset++;

        return indexOffset;
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is not IDynamicColumn<TModel> column || e.PropertyName != nameof(IDynamicColumn.Visible)) return;

        if (column.Visible && !_displayedColumns.Contains(column)) {
            var index = IndexOf(column);
            var offset = GetDisplayedOffset(index);
            _displayedColumns.Insert(index - offset, column);
        }
        else if (_displayedColumns.Count <= 1) {
            column.Visible = true;
        }
        else {
            _displayedColumns.Remove(column);
        }
    }
}
