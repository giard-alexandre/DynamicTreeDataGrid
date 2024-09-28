namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumns : IDynamicColumnsBase {
	public IDynamicColumnsBase DisplayedColumns { get; }
}
