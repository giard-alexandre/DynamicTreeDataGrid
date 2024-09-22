using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Experimental.Data;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Models;

public abstract class DynamicColumnBase<TModel, TValue> : ColumnBase<TModel, TValue>, IDynamicColumn<TModel>
	where TModel : class {
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicColumnBase{TModel, TValue}"/> class.
	/// </summary>
	/// <param name="name">
	/// Unique name for the column.
	/// Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource"/>
	/// </param>
	/// <param name="header">The column header.</param>
	/// <param name="getter">
	/// An expression which given a row model, returns a cell value for the column.
	/// </param>
	/// <param name="setter">
	/// A method which given a row model and a cell value, writes the cell value to the
	/// row model. If null, the column will be read-only.
	/// </param>
	/// <param name="width">
	/// The column width. If null defaults to <see cref="GridLength.Auto"/>.
	/// </param>
	/// <param name="options">Additional column options.</param>
	protected DynamicColumnBase(string name,
	                            object? header,
	                            Expression<Func<TModel, TValue?>> getter,
	                            Action<TModel, TValue?>? setter,
	                            GridLength? width,
	                            ColumnOptions<TModel> options) : base(header, getter, setter, width, options) {
		Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicColumnBase{TModel, TValue}"/> class.
	/// </summary>
	/// <param name="name">
	/// Unique name for the column.
	/// Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource"/>
	/// </param>
	/// <param name="header">The column header.</param>
	/// <param name="valueSelector">
	/// the function which selects the column value from the model..
	/// </param>
	/// <param name="binding">
	/// a binding which selects the column value from the model.
	/// </param>
	/// <param name="width">
	/// The column width. If null defaults to <see cref="GridLength.Auto"/>.
	/// </param>
	/// <param name="options">Additional column options.</param>
	protected DynamicColumnBase(string name,
	                            object? header,
	                            Func<TModel, TValue?> valueSelector,
	                            TypedBinding<TModel, TValue?> binding,
	                            GridLength? width,
	                            ColumnOptions<TModel>? options) : base(header, valueSelector, binding, width, options) {
		Name = name;
	}

	public string Name { get; set; }

	public bool Visible { get; set; } = true;
}
