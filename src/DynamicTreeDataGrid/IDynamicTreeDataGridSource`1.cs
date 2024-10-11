using Avalonia.Controls;

using DynamicTreeDataGrid.State;

namespace DynamicTreeDataGrid;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {

    // TODO: Add filter, state, maybe sort?
    IEnumerable<ColumnState> GetColumnStates();

    /// <summary>
    /// Applies the supplied column state to the sources' column list.
    /// </summary>
    /// <param name="states">The states to apply</param>
    /// <returns><c>true</c> if applied successfully, <c>false</c> if any errors occurred.</returns>
    bool ApplyColumnStates(IEnumerable<ColumnState> states);
}
