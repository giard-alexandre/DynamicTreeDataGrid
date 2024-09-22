using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Models;

public abstract class DynamicColumnBase<TModel> : ColumnBase<TModel>, IDynamicColumn<TModel> {
    public DynamicColumnBase(object? header, GridLength? width, ColumnOptions<TModel> options) : base(header, width,
        options) { }

    public bool Visible { get; set; } = true;
}
