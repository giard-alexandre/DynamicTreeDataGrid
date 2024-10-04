namespace DynamicTreeDataGrid;

/// <summary>
/// A comparer that doesn't affect the comparison at all (will not change the order of items)
/// </summary>
/// <typeparam name="T"></typeparam>
public class NoSortComparer<T> : IComparer<T> {
    public int Compare(T? x, T? y) => 0;
}
