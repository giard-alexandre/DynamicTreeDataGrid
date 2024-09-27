using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel> {

    // TODO: Add filter, state, maybe sort?
}
