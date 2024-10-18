namespace DynamicTreeDataGrid.Models.State;

public record GridState {
	public IList<ColumnState> ColumnStates { get; set; } = [];
}
