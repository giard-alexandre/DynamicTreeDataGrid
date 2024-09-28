using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumn<TModel> : IColumn<TModel>, IDynamicColumn;
