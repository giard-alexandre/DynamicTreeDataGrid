using Avalonia.Controls.Models.TreeDataGrid;

namespace DynamicTreeDataGrid.Models;

public interface IFilterableColumn<TModel> : IColumn<TModel> {
    public bool Visible { get; set; }

    // Use a Predicate?
    // Something?
    public object Filter { get; set; }
}
