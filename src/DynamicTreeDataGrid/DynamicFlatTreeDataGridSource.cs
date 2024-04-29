using System.Collections.ObjectModel;

using Avalonia.Controls;

using DynamicData;

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel, TModelKey> : FlatTreeDataGridSource<TModel>
	where TModel : class where TModelKey : notnull {
	private readonly ObservableCollection<TModel> _items = [];

	public DynamicFlatTreeDataGridSource(IChangeSet<TModel, TModelKey> items) : base(Enumerable.Empty<TModel>()) {
		// items.Bind(_items)
	}
}
