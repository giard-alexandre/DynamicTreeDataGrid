﻿using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

namespace DynamicTreeDataGrid.Models.Columns;

/// <summary>
/// A column in an <see cref="ITreeDataGridSource"/> which displays a check box.
/// Direct port of <see cref="CheckBoxColumn{TModel}"/> that adds filtering and visbility.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public class FilterableCheckBoxColumn<TModel> : FilterableColumnBase<TModel, bool?>
    where TModel : class {
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterableCheckBoxColumn{TModel}"/> class.
    /// Direct port of <see cref="CheckBoxColumn{TModel}"/> that adds filtering and visbility.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    /// An expression which given a row model, returns a boolean cell value for the column.
    /// </param>
    /// <param name="setter">
    /// A method which given a row model and a cell value, writes the cell value to the
    /// row model. If not supplied then the column will be read-only.
    /// </param>
    /// <param name="width">
    /// The column width. If null defaults to <see cref="GridLength.Auto"/>.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public FilterableCheckBoxColumn(object? header,
                                    Expression<Func<TModel, bool>> getter,
                                    Action<TModel, bool>? setter = null,
                                    GridLength? width = null,
                                    CheckBoxColumnOptions<TModel>? options = null) : base(header, ToNullable(getter),
        ToNullable(getter, setter), width, options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterableCheckBoxColumn{TModel}"/> class that
    /// displays a three-state check box.
    /// Direct port of <see cref="CheckBoxColumn{TModel}"/> that adds filtering and visbility.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    /// An expression which given a row model, returns a nullable boolean cell value for the
    /// column.
    /// </param>
    /// <param name="setter">
    /// A method which given a row model and a cell value, writes the cell value to the
    /// row model. If not supplied then the column will be read-only.
    /// </param>
    /// <param name="width">
    /// The column width. If null defaults to <see cref="GridLength.Auto"/>.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public FilterableCheckBoxColumn(object? header,
                                    Expression<Func<TModel, bool?>> getter,
                                    Action<TModel, bool?>? setter = null,
                                    GridLength? width = null,
                                    CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
        options ?? new()) {
        IsThreeState = true;
    }

    public bool IsThreeState { get; }

    public override ICell CreateCell(IRow<TModel> row) {
        return new CheckBoxCell(CreateBindingExpression(row.Model), Binding.Write is null, IsThreeState);
    }

    private static Func<TModel, bool?> ToNullable(Expression<Func<TModel, bool>> getter) {
        var c = getter.Compile();
        return x => c(x);
    }

    private static TypedBinding<TModel, bool?> ToNullable(Expression<Func<TModel, bool>> getter,
                                                          Action<TModel, bool>? setter) {
        var g = Expression.Lambda<Func<TModel, bool?>>(Expression.Convert(getter.Body, typeof(bool?)),
            getter.Parameters);

        return setter is null
            ? TypedBinding<TModel>.OneWay(g)
            : TypedBinding<TModel>.TwoWay(g, (m, v) => setter(m, v ?? false));
    }
}
