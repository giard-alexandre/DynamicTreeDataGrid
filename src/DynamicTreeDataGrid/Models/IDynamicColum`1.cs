using Avalonia.Controls.Models.TreeDataGrid;

using DynamicTreeDataGrid.State;

namespace DynamicTreeDataGrid.Models;

public interface IDynamicColum<TModel> : IColumn<TModel> {
	public ColumnState ColumnState { get; set; }
}
