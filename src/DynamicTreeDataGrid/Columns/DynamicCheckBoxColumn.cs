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
/// Basically a <seealso cref="CheckBoxColumn{TModel}"/> but with the required <see cref="Name"/> field ti make this
/// packages features work.
/// </remarks>
public class DynamicCheckBoxColumn<TModel> : CheckBoxColumn<TModel>, IDynamicColumn<TModel>
	where TModel : class {
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicCheckBoxColumn{TModel}"/> class.
	/// </summary>
	/// <param name="name">
	/// Unique name for the column.
	/// Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource"/>
	/// </param>
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
	public DynamicCheckBoxColumn(string name,
	                             object? header,
	                             Expression<Func<TModel, bool>> getter,
	                             Action<TModel, bool>? setter = null,
	                             GridLength? width = null,
	                             CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
		options) {
		Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicCheckBoxColumn{TModel}"/> class that
	/// displays a three-state check box.
	/// </summary>
	/// <param name="name">
	/// Unique name for the column.
	/// Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource"/>
	/// </param>
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
	public DynamicCheckBoxColumn(string name,
	                             object? header,
	                             Expression<Func<TModel, bool?>> getter,
	                             Action<TModel, bool?>? setter = null,
	                             GridLength? width = null,
	                             CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
		options) {
		Name = name;
	}

	public string Name { get; init; }
}
