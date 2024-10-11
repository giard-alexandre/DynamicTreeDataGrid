using Avalonia.Controls;

using DynamicTreeDataGrid.State;

namespace DynamicTreeDataGrid;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {

    // TODO: Add filter, state, maybe sort?
    IEnumerable<ColumnState> GetColumnStates();
}
