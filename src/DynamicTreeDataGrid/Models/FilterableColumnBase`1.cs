using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models;

public abstract class FilterableColumnBase<TModel> : ColumnBase<TModel>, IFilterableColumn<TModel> {
    public FilterableColumnBase(object? header, GridLength? width, ColumnOptions<TModel> options) : base(header, width,
        options) { }


    /// <summary>
    /// Whether the column should be visible in the table. Defaults to <c>true</c>
    /// </summary>
    public bool Visible { get; set; } = true;

    public object Filter { get; set; } = new();
}
