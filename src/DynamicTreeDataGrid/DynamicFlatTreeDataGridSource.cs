using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel> : FlatTreeDataGridSource<TModel>,
	IDynamicTreeDataGridSource<TModel>
	where TModel : class {
	public DynamicFlatTreeDataGridSource(IEnumerable<TModel> items) : base(items) { }
}
