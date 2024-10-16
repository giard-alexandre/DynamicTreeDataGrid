using Avalonia.Controls;

namespace DynamicTreeDataGrid.Source;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {
    // TODO: Add filter, state, maybe sort?
}
