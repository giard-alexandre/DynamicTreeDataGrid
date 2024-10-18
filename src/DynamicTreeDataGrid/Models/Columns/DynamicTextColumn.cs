using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public class DynamicTextColumn<TModel, TValue> : TextColumn<TModel, TValue>, IDynamicColumn<TModel>
    where TModel : class {
    private bool _visible = true;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextColumn{TModel, TValue}" /> class.
    /// </summary>
    /// <param name="name">
    ///     Unique name for the column.
    ///     Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource" />
    /// </param>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    ///     An expression which given a row model, returns a cell value for the column.
    /// </param>
    /// <param name="width">
    ///     The column width. If null defaults to <see cref="GridLength.Auto" />.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public DynamicTextColumn(string name,
        object? header,
        Expression<Func<TModel, TValue?>> getter,
        GridLength? width = null,
        TextColumnOptions<TModel>? options = null) : base(header, getter, width, options) {
        Name = name;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextColumn{TModel, TValue}" /> class.
    /// </summary>
    /// <param name="name">
    ///     Unique name for the column.
    ///     Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource" />
    /// </param>
    /// <param name="header">The column header.</param>
    /// <param name="getter">
    ///     An expression which given a row model, returns a cell value for the column.
    /// </param>
    /// <param name="setter">
    ///     A method which given a row model and a cell value, writes the cell value to the
    ///     row model.
    /// </param>
    /// <param name="width">
    ///     The column width. If null defaults to <see cref="GridLength.Auto" />.
    /// </param>
    /// <param name="options">Additional column options.</param>
    public DynamicTextColumn(string name,
        object? header,
        Expression<Func<TModel, TValue?>> getter,
        Action<TModel, TValue?> setter,
        GridLength? width = null,
        TextColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
        options ?? new TextColumnOptions<TModel>()) {
        Name = name;
    }

    public string Name { get; init; }

    public bool Visible {
        get => _visible;
        set {
            if (value == _visible) return;
            _visible = value;
            RaisePropertyChanged();
        }
    }
}
