using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumns : IColumns, IReadOnlyList<IDynamicColumn>;
