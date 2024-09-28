using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumnsBase : IColumns, IReadOnlyList<IDynamicColumn>;
