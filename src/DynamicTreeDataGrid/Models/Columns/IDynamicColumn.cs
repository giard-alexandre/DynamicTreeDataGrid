using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumn : IColumn {

	/// <summary>
	/// Whether the column should be visible in the table. Defaults to <c>true</c>
	/// </summary>
	public bool Visible { get; set; }
}
