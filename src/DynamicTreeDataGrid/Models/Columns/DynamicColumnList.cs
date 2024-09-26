using System.Collections.Specialized;
using System.ComponentModel;

namespace DynamicTreeDataGrid.Models.Columns;

/// <summary>
/// Maintains 2 lists of columns. The main one, which is exposed by default, is the list of all columns
/// defined. The second, exposed through "DisplayedColumns" is the list that is shown to the user (Visible).
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class DynamicColumnList<TModel> : DynamicColumnListBase<TModel> {
    /// <summary>
    /// Columns that are used by the TreeDataGrid. Filters out the columns that should not be visible.
    /// </summary>
    public DynamicColumnListBase<TModel> DisplayedColumns { get; } = [];

    public DynamicColumnList() {
        this.CollectionChanged += SyncFilteredCollection;

        // Subscribe to property changes for initial items
        foreach (var item in this) {
            ItemAdded(item);
        }
    }


    private void SyncFilteredCollection(object? sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                foreach (var newItem in e.NewItems) {
                    ItemAdded(newItem);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var oldItem in e.OldItems) {
                    ItemRemoved(oldItem);
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                ItemReplaced(e);

                break;

            case NotifyCollectionChangedAction.Move:
                // Handle move if needed
                break;

            case NotifyCollectionChangedAction.Reset:
                // Just clear the Displayed ones, we're re-adding everything anyways.
                DisplayedColumns.Clear();
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
        if (column.Visible) {
            DisplayedColumns.Add(column);
        }
    }

    private void ItemRemoved(object? item) {
        if (item is not IDynamicColumn<TModel> column) return;
        column.PropertyChanged -= Item_PropertyChanged;
        DisplayedColumns.Remove(column);
    }

    private void ItemReplaced(NotifyCollectionChangedEventArgs e) {
        throw new NotImplementedException("Replace is not implemented yet");

        // for (int i = 0; i < e.OldItems.Count; i++) {
        //     DisplayedColumns[e.OldStartingIndex + i] = (IDynamicColumn<TModel>)e.NewItems[i];
        // }
        // Unsubscribe from old item changes
        // Subscribe to new item changes
        // Replace in the filtered list too (somehow)
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is IDynamicColumn<TModel> column && e.PropertyName == nameof(IDynamicColumn.Visible)) {
            if (column.Visible && !DisplayedColumns.Contains(column)) {
                DisplayedColumns.Add(column);
            }
            else {
                DisplayedColumns.Remove(column);
            }
        }
    }
}
