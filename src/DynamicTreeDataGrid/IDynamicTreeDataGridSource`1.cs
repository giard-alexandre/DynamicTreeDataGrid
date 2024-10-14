using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {
    // TODO: Add filter, state, maybe sort?
}
