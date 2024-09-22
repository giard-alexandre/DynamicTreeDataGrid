using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Models;

public abstract class DynamicColumnBase<TModel, TValue> : ColumnBase<TModel, TValue>, IDynamicColumn<TModel>
    where TModel : class {
    /// <inheritdoc />
    protected DynamicColumnBase(object? header,
                                   Expression<Func<TModel, TValue?>> getter,
                                   Action<TModel, TValue?>? setter,
                                   GridLength? width,
                                   ColumnOptions<TModel> options) : base(header, getter, setter, width, options) { }

    /// <inheritdoc />
    protected DynamicColumnBase(object? header,
                                   Func<TModel, TValue?> valueSelector,
                                   TypedBinding<TModel, TValue?> binding,
                                   GridLength? width,
                                   ColumnOptions<TModel>? options) : base(header, valueSelector, binding, width,
        options) { }

    /// <summary>
    /// Whether the column should be visible in the table. Defaults to <c>true</c>
    /// </summary>
    public bool Visible { get; set; } = true;
}
