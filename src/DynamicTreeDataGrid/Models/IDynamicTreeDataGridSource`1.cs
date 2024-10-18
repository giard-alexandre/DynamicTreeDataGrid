using Avalonia.Controls;

namespace DynamicTreeDataGrid.Models;

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel>, IDynamicTreeDataGridSource {
    public DynamicTreeDataGridSourceOptions<TModel> Options { get; }
}
