using Avalonia;
using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public class DynamicTreeDataGrid : TreeDataGrid {
    protected override Type StyleKeyOverride => typeof(TreeDataGrid);


    public new static readonly DirectProperty<DynamicTreeDataGrid, IDynamicTreeDataGridSource?> SourceProperty =
        AvaloniaProperty.RegisterDirect<DynamicTreeDataGrid, IDynamicTreeDataGridSource?>(
            nameof(Source),
            o => o.Source,
            (o, v) => o.Source = v);



    public new IDynamicTreeDataGridSource? Source
    {
        get => (IDynamicTreeDataGridSource?)base.Source;
        set => base.Source = value;
    }


    public void ShowColumnEditor() {
        var columnsWindow = new Window();
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window) {
            columnsWindow.ShowDialog(window);
        }
    }
}
