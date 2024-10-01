namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumns : IDynamicColumnsBase {
	public IDynamicColumnsBase DisplayedColumns { get; }
	public void Move(int oldIndex, int newIndex);
	public void Move(IDynamicColumn oldLocation, IDynamicColumn newLocation);

}
