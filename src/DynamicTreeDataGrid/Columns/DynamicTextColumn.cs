using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Columns;

public class DynamicTextColumn<TModel, TValue> : FilterableTextColumn<TModel, TValue>
    where TModel : class {
    public DynamicTextColumn(object? header,
                             Expression<Func<TModel, TValue?>> getter,
                             Action<TModel, TValue?>? setter,
                             GridLength? width,
                             ColumnOptions<TModel> options) : base(header, getter, setter, width, options) { }

    public DynamicTextColumn(object? header,
                             Func<TModel, TValue?> valueSelector,
                             TypedBinding<TModel, TValue?> binding,
                             GridLength? width,
                             ColumnOptions<TModel>? options) : base(header, valueSelector, binding, width, options) { }
}
