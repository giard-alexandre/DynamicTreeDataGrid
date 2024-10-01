using System.Diagnostics.CodeAnalysis;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Controls;

public partial class ColumnListView : UserControl {
    private const string DragItemFormat = "icolumn-item-format";
    private readonly Point _mouseOffset = new(-5, -5);

    private Border? _draggedItem;

    public ColumnListView() {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private async void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        Console.WriteLine("DoDrag start");

        if (sender is not Border border) return;
        if (border.DataContext is not IColumn column) return;

        _draggedItem = border;

        // var ghostPos = GhostItem.Bounds.Position;
        // _ghostPosition = new Point(ghostPos.X + _mouseOffset.X, ghostPos.Y + _mouseOffset.Y);

        // var mousePos = e.GetPosition(MainContainer);
        // var offsetX = mousePos.X - ghostPos.X;
        // var offsetY = mousePos.Y - ghostPos.Y + _mouseOffset.X;
        // GhostItem.RenderTransform = new TranslateTransform(offsetX, offsetY);

        // if (DataContext is not DragAndDropPageViewModel vm) return;
        // vm.StartDrag(column);

        // GhostItem.IsVisible = true;

        var dragData = new DataObject();
        dragData.Set(DragItemFormat, column);
        var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        var finalPosition = e.GetPosition(MainContainer);
        Console.WriteLine($"DragAndDrop result: {result}");

        // GhostItem.IsVisible = false;
    }

    private void DragOver(object? sender, DragEventArgs e) {
        Console.WriteLine("DragOver");
        Console.WriteLine(sender);
        Console.WriteLine(e.DragEffects);

        //
        // GhostItem.RenderTransform = new TranslateTransform(offsetX, offsetY);

        // set drag cursor icon
        e.DragEffects = DragDropEffects.Move;

        // if (DataContext is not DragAndDropPageViewModel vm) return;
        var data = e.Data.Get(DragItemFormat);
        if (data is not IColumn column) return;

        // if (!vm.IsDestinationValid(taskItem, (e.Source as Control)?.Name))
        // {
        //     e.DragEffects = DragDropEffects.None;
        // }

        var currentPosition = e.GetPosition(MainContainer);

        if (_draggedItem is null) return;
        var offsetX = currentPosition.X - _draggedItem.Bounds.Position.X;
        var offsetY = currentPosition.Y - _draggedItem.Bounds.Position.Y;
        _draggedItem.RenderTransform = new TranslateTransform(offsetX, offsetY);

    }

    private void Drop(object? sender, DragEventArgs e) {
        Console.WriteLine("Drop");

        var data = e.Data.Get(DragItemFormat);

        if (data is not IDynamicColumn sourceColumn) {
            Console.WriteLine("No task item");
            return;
        }

        if (e.Source is not Control control || !TryGetColumn(control, out var targetColumn)) {
            Console.WriteLine("Invalid Drop Target");
            return;
        }

        if (DataContext is IDynamicColumns collection) {
            collection.Move(sourceColumn, targetColumn);
        }

        // Remove dragged item transform if its set.
        if (_draggedItem is not null) {
            _draggedItem.RenderTransform = null;
        }
    }

    private static bool TryGetColumn(Control? control, [MaybeNullWhen(false)] out IDynamicColumn column) {
        column = null;
        switch (control) {
            case null:
                return false;
            case Border { DataContext: IDynamicColumn col }:
                column = col;
                return true;
            default: {
                var itemControl = control.GetVisualAncestors()
                    .OfType<Border>()
                    .FirstOrDefault(x => x.DataContext is IDynamicColumn);
                if (itemControl is not Border { DataContext: IDynamicColumn col }) return false;
                column = col;
                return true;
            }
        }
    }
}
