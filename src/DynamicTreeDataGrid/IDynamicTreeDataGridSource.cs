using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public interface IDynamicTreeDataGridSource : ITreeDataGridSource {

}

public interface IDynamicTreeDataGridSource<TModel> : ITreeDataGridSource<TModel> {

}
