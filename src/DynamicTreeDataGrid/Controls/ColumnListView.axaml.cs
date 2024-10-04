using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Controls;

public partial class ColumnListView : UserControl {
    private const string DragItemFormat = "icolumn-item-format";

    private ColumnItemView? _draggedItem;

    public ColumnListView() {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private async void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        // We don't care about anything that isn't one of our list items.
        if (sender is not ColumnItemView border) return;
        if (border.DataContext is not IColumn column) return;

        _draggedItem = border;
        // Disable Hit test otherwise this control will be the drop target since the mouse is over it.
        _draggedItem.IsHitTestVisible = false;

        var dragData = new DataObject();
        dragData.Set(DragItemFormat, column);
        await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
    }

    private void DragOver(object? sender, DragEventArgs e) {
        // set drag cursor icon
        e.DragEffects = DragDropEffects.Move;

        if (_draggedItem is null) return;

        // Render the item under the mouse and follow the mouse.
        var currentPosition = e.GetPosition(MainContainer);
        var offsetX = currentPosition.X - _draggedItem.Bounds.Position.X - _draggedItem.Bounds.Width/2;
        var offsetY = currentPosition.Y - _draggedItem.Bounds.Position.Y - _draggedItem.Bounds.Height/2;
        _draggedItem.RenderTransform = new TranslateTransform(offsetX, offsetY);

    }

    private void Drop(object? sender, DragEventArgs e) {
        var data = e.Data.Get(DragItemFormat);

        if (data is not IDynamicColumn sourceColumn) {
            Console.WriteLine("No column item");
            return;
        }

        // Remove dragged item transform if its set.
        if (_draggedItem is not null) {
            _draggedItem.RenderTransform = null;
            _draggedItem.IsHitTestVisible = true;
        }

        // Get the list element where we dropped the source element. (e.Source is the "target" in this case)
        if (e.Source is not Control control || !TryGetColumn(control, out var targetColumn)) {
            Console.WriteLine("Invalid Drop Target");
            return;
        }

        // Move the item in the collection
        if (DataContext is IDynamicColumns collection) {
            collection.Move(sourceColumn, targetColumn);
        }
    }

    private static bool TryGetColumn(Control? control, [MaybeNullWhen(false)] out IDynamicColumn column) {
        column = null;
        switch (control) {
            case null:
                return false;
            case ColumnItemView { DataContext: IDynamicColumn col }:
                column = col;
                return true;
            default: {
                var itemControl = control.GetVisualAncestors()
                    .OfType<ColumnItemView>()
                    .FirstOrDefault(x => x.DataContext is IDynamicColumn);
                if (itemControl is not ColumnItemView { DataContext: IDynamicColumn col }) return false;
                column = col;
                return true;
            }
        }
    }

    private void ShowAllClicked(object? sender, RoutedEventArgs e) {
        if(DataContext is not IDynamicColumns columns) return;
        // for (int i = 0; i < columns.Count; i++) {
        //
        // }
    }
    private void HideAllClicked(object? sender, RoutedEventArgs e) { throw new NotImplementedException(); }
}
