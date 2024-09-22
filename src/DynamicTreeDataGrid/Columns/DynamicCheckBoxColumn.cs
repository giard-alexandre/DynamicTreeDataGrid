using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Columns;

/// <summary>
/// A column in an <see cref="ITreeDataGridSource"/> which displays a check box.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <remarks>
/// Extra abstraction over <seealso cref="Models.Columns.DynamicCheckBoxColumn{TModel}"/> as that class is meant to copy
/// <seealso cref="CheckBoxColumn{TModel}"/> as closely as possible. This class adds all of this packages
/// extra features to it!
/// </remarks>
public class DynamicCheckBoxColumn<TModel> : DynamicCheckBoxColumnBase<TModel>
	where TModel : class {
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicCheckBoxColumnBase{TModel}"/> class.
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
	public DynamicCheckBoxColumn(object? header,
	                             Expression<Func<TModel, bool>> getter,
	                             Action<TModel, bool>? setter = null,
	                             GridLength? width = null,
	                             CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
		options) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicCheckBoxColumnBase{TModel}"/> class that
	/// displays a three-state check box.
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
	public DynamicCheckBoxColumn(object? header,
	                             Expression<Func<TModel, bool?>> getter,
	                             Action<TModel, bool?>? setter = null,
	                             GridLength? width = null,
	                             CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
		options) { }
}
