using Avalonia.Controls;

namespace DynamicTreeDataGrid.Models;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {
    // TODO: Add filter, state, maybe sort?
}
