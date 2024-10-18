using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using DynamicTreeDataGrid.Models.Sorting;

namespace DynamicTreeDataGrid;

public class DynamicTreeDataGridSourceOptions<TModel> {
    /// <summary>
    /// Sort to apply BEFORE the requested column sort.
    /// Think of this as: <c>Sort(PreColumnSort.ThenBy(ColumnSort).ThenBy(PostColumnSort)</c>
    /// </summary>
    public IComparer<TModel> PreColumnSort { get; set; } = new NoSortComparer<TModel>();

    /// <summary>
    /// Sort to apply AFTER the requested column sort.
    /// Think of this as: <c>Sort(PreColumnSort.ThenBy(ColumnSort).ThenBy(PostColumnSort)</c>
    /// </summary>
    public IComparer<TModel> PostColumnSort { get; set; } = new NoSortComparer<TModel>();

    /// <summary>
    /// Sort optimization setting to apply to ALL sorts.
    /// </summary>
    /// <remarks>
    /// Includes the pre-columns and post-column sorts as well as the column sort itself.
    /// You must be VERY sure that all sort possibilities can adhere to the <see cref="SortOptimisations"/> selected.<br/>
    /// If any column has sorting enabled and the values are not immutable, then don't select
    /// <see cref="DynamicData.SortOptimisations.ComparesImmutableValuesOnly"/>, for exmaple.
    /// </remarks>
    public SortOptimisations SortOptimisations { get; init; } = SortOptimisations.None;

    /// <summary>
    /// Observable that can be used to re-trigger sorting if either <see cref="PreColumnSort"/>
    /// or <see cref="PostColumnSort"/> changes.
    /// </summary>
    public IObservable<Unit> Resorter { get; init; } = Observable.Empty<Unit>();
}
