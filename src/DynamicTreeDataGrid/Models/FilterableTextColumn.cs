using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

namespace DynamicTreeDataGrid.Models;

public class FilterableTextColumn<TModel, TValue> : FilterableColumnBase<TModel, TValue>, ITextSearchableColumn<TModel>
    where TModel : class {
    private ITextSearchableColumn<TModel> _textSearchableColumnImplementation;

    // TODO: Look into using <inheritdoc>?
    /// <summary>
    /// Initializes a new instance of the <see cref="TextColumn{TModel, TValue}"/> class.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    /// An expression which given a row model, returns a cell value for the column.
    /// </param>
    /// <param name="width">
    /// The column width. If null defaults to <see cref="GridLength.Auto"/>.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public FilterableTextColumn(object? header,
                                Expression<Func<TModel, TValue?>> getter,
                                Action<TModel, TValue?>? setter,
                                GridLength? width,
                                ColumnOptions<TModel> options) : base(header, getter, setter, width, options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextColumn{TModel, TValue}"/> class.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    /// An expression which given a row model, returns a cell value for the column.
    /// </param>
    /// <param name="setter">
    /// A method which given a row model and a cell value, writes the cell value to the
    /// row model.
    /// </param>
    /// <param name="width">
    /// The column width. If null defaults to <see cref="GridLength.Auto"/>.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public FilterableTextColumn(object? header,
                                Func<TModel, TValue?> valueSelector,
                                TypedBinding<TModel, TValue?> binding,
                                GridLength? width,
                                ColumnOptions<TModel>? options) : base(header, valueSelector, binding, width,
        options) { }

    public new TextColumnOptions<TModel> Options => (TextColumnOptions<TModel>)base.Options;

    bool ITextSearchableColumn<TModel>.IsTextSearchEnabled => Options?.IsTextSearchEnabled ?? false;

    public override ICell CreateCell(IRow<TModel> row)
    {
        return new TextCell<TValue?>(CreateBindingExpression(row.Model), Binding.Write is null, Options);
    }

    string? ITextSearchableColumn<TModel>.SelectValue(TModel model)
    {
        return ValueSelector(model)?.ToString();
    }
}
