using Avalonia.Controls;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Models;

public interface IDynamicTreeDataGridSource : ITreeDataGridSource {
    new IDynamicColumns Columns { get; }

    /// <summary>
    ///     The amount of items in the table, once filtered.
    /// </summary>
    public IObservable<int> FilteredCount { get; }

    /// <summary>
    ///     The total amount of items in the table. (before filtering is applied).
    /// </summary>
    public IObservable<int> TotalCount { get; }

    // TODO: Add filter, state, maybe sort?
}
