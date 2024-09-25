using System.Collections.Specialized;
using System.ComponentModel;

namespace DynamicTreeDataGrid.Models.Columns;

/// <summary>
/// Maintains 2 lists of columns. The main one, which is exposed by default, is the list of all columns
/// defined. The second, exposed through "DisplayedColumns" is the list that is shown to the user (Visible).
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class DynamicColumnList<TModel> : DynamicColumnListBase<TModel> {
    public DynamicColumnList() {
        this.CollectionChanged += SyncFilteredCollection;

        // Subscribe to property changes for initial items
        foreach (var item in this) {
            ItemAdded(item);
        }
    }

    public DynamicColumnListBase<TModel> DisplayedColumns { get; } = [];

    private void SyncFilteredCollection(object? sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                foreach (var newItem in e.NewItems) {
                    ItemAdded(newItem);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var oldItem in e.OldItems) {
                    DisplayedColumns.Remove((IDynamicColumn<TModel>)oldItem);
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                for (int i = 0; i < e.OldItems.Count; i++) {
                    DisplayedColumns[e.OldStartingIndex + i] = (IDynamicColumn<TModel>)e.NewItems[i];
                }

                break;

            case NotifyCollectionChangedAction.Move:
                // Handle move if needed
                break;

            case NotifyCollectionChangedAction.Reset:
                DisplayedColumns.Clear();
                foreach (var column in this) {
                    if (column.Visible) {
                        DisplayedColumns.Add(column);
                    }
                }

                break;
        }
    }

    private void ItemAdded(object? item) {
        if (item is not IDynamicColumn<TModel> column) return;
        SubscribeToItemPropertyChanged(column);
        if (column.Visible) {
            DisplayedColumns.Add(column);
        }
    }


    private static void SubscribeToItemPropertyChanged(IDynamicColumn<TModel> item) {
        item.PropertyChanged += Item_PropertyChanged;
    }

    private static void UnsubscribeFromItemPropertyChanged(IDynamicColumn<TModel> item) {
        item.PropertyChanged -= Item_PropertyChanged;
    }

    private static void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is IDynamicColumn<TModel> item) {
            // TODO: Remove/Add from list if visible is changed
            Console.WriteLine($"Property '{e.PropertyName}' of item '{item.Name}' has changed to {item}");
        }
    }
}
