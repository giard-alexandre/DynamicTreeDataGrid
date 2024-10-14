namespace DynamicTreeDataGrid.State;

public record ColumnState(string Name) {
    public string Name { get; init; } = Name;
    public int Index { get; set; }
    public bool Visible { get; set; } = true;
}
