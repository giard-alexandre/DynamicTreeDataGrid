using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Models;

public abstract class DynamicColumnBase<TModel> : ColumnBase<TModel>, IDynamicColumn<TModel> {
	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnBase{TModel, TValue}"/> class.
	/// </summary>
	/// <param name="name">
	/// Unique name for the column.
	/// Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource"/>
	/// </param>
	/// <param name="header">The column header.</param>
	/// <param name="width">
	/// The column width. If null defaults to <see cref="GridLength.Auto"/>.
	/// </param>
	/// <param name="options">Additional column options.</param>
	protected DynamicColumnBase(string name, object? header, GridLength? width, ColumnOptions<TModel> options) : base(
		header, width, options) {
		Name = name;
	}

	public string Name { get; set; }
	public bool Visible { get; set; } = true;
}
