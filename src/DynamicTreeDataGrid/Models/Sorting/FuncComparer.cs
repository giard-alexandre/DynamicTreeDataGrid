// Code From: https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid/blob/master/src/Avalonia.Controls.TreeDataGrid/Models/TreeDataGrid/FuncComparer.cs

namespace DynamicTreeDataGrid.Models.Sorting;

internal class FuncComparer<T> : IComparer<T> {
    private readonly Comparison<T?> _func;
    public FuncComparer(Comparison<T?> func) { _func = func; }

    public int Compare(T? x, T? y) => _func(x, y);
}
