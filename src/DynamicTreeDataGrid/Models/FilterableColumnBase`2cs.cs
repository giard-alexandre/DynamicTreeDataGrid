using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

namespace DynamicTreeDataGrid.Models;

public abstract class FilterableColumnBase<TModel, TValue> : ColumnBase<TModel, TValue>, IFilterableColumn<TModel>
    where TModel : class {
    protected FilterableColumnBase(object? header,
                                   Expression<Func<TModel, TValue?>> getter,
                                   Action<TModel, TValue?>? setter,
                                   GridLength? width,
                                   ColumnOptions<TModel> options) : base(header, getter, setter, width, options) { }

    protected FilterableColumnBase(object? header,
                                   Func<TModel, TValue?> valueSelector,
                                   TypedBinding<TModel, TValue?> binding,
                                   GridLength? width,
                                   ColumnOptions<TModel>? options) : base(header, valueSelector, binding, width,
        options) { }

    /// <summary>
    /// Whether the column should be visible in the table. Defaults to <c>true</c>
    /// </summary>
    public bool Visible { get; set; } = true;

    public object Filter { get; set; } = new();
}
