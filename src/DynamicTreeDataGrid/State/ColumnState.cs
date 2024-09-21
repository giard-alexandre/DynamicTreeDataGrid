namespace DynamicTreeDataGrid.State;

public class ColumnState {
    public Guid Id { get; }
    public int Index { get; set; }
    public bool Visible { get; set; }

    public ColumnState(Guid id) { Id = id; }
}
