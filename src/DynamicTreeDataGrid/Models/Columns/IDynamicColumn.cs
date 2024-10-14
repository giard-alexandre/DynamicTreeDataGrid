using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumn : IColumn {
	/// <summary>
	///     Unique name for the column.
	///     Must be unique for each column in the <see cref="Avalonia.Controls.ITreeDataGridSource" />
	/// </summary>
	public string Name { get; init; }

    // /// <summary>
    // /// Whether the column should be visible in the table. Defaults to <c>true</c>
    // /// </summary>
    public bool Visible { get; set; }
}
