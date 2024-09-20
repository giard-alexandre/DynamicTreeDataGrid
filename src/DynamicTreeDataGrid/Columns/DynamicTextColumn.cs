using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Columns;

public class DynamicTextColumn<TModel, TValue> : TextColumn<TModel, TValue>
    where TModel : class {
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
    public DynamicTextColumn(object? header,
                             Expression<Func<TModel, TValue?>> getter,
                             GridLength? width = null,
                             TextColumnOptions<TModel>? options = null) : base(header, getter, width, options) { }

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
    public DynamicTextColumn(object? header,
                             Expression<Func<TModel, TValue?>> getter,
                             Action<TModel, TValue?> setter,
                             GridLength? width = null,
                             TextColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
        options ?? new TextColumnOptions<TModel>()) { }
}
