namespace DynamicTreeDataGrid.Models.Sorting;

internal sealed class CombinedComparer<T>(
    IComparer<T>? primaryComparer = null,
    IComparer<T>? secondaryComparer = null,
    IComparer<T>? tertiaryComparer = null) : IComparer<T> {
    public int Compare(T? x, T? y) {
        int result = primaryComparer?.Compare(x, y) ?? 0;
        if (result == 0) {
            result = secondaryComparer?.Compare(x, y) ?? 0;
        }

        if (result == 0) {
            result = tertiaryComparer?.Compare(x, y) ?? 0;
        }

        return result;
    }
}
