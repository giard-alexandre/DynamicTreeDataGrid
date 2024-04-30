using Avalonia.Controls;

using DynamicData;

namespace DynamicTreeDataGrid;

public class DynamicHierarchicalTreeDataGridSource<TModel> : HierarchicalTreeDataGridSource<TModel>,
	IDynamicTreeDataGridSource<TModel>
	where TModel : class {
	public DynamicHierarchicalTreeDataGridSource(IChangeSet<TModel, TModelKey> changeStream) : base(item) {
	}
	public DynamicHierarchicalTreeDataGridSource(IEnumerable<TModel> items) : base(items) { }
}
