using System.ComponentModel;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;

using DynamicData;
using DynamicData.Aggregation;

namespace DynamicTreeDataGrid;

public class DynamicFlatTreeDataGridSource<TModel, TModelKey> : ITreeDataGridSource<TModel>
	where TModel : class where TModelKey : notnull {
	private readonly FlatTreeDataGridSource<TModel> treeDataGridSourceImplementation;

	public DynamicFlatTreeDataGridSource(IObservable<IChangeSet<TModel, TModelKey>> changes) {
		//TODO: Consider RefCount?
		var unfilteredCount = changes.Count();
		var filteredChanges = changes
			// .Filter(model => model) // Consider using DynamicData.PLinq for filtering.
			.Do(_ => Console.WriteLine("FILTERINGGGGGGGGG"));

		var myOperation = filteredChanges
			// .Filter()

			// .Filter(trade=>trade.Status == TradeStatus.Live)
			// .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp))
			.Bind(out var list)
			.DisposeMany()
			.Subscribe();

		treeDataGridSourceImplementation = new FlatTreeDataGridSource<TModel>(list);
	}

	public event PropertyChangedEventHandler? PropertyChanged {
		add => treeDataGridSourceImplementation.PropertyChanged += value;
		remove => treeDataGridSourceImplementation.PropertyChanged -= value;
	}

	public void DragDropRows(ITreeDataGridSource source,
	                         IEnumerable<IndexPath> indexes,
	                         IndexPath targetIndex,
	                         TreeDataGridRowDropPosition position,
	                         DragDropEffects effects) {
		((ITreeDataGridSource)treeDataGridSourceImplementation).DragDropRows(source, indexes, targetIndex, position, effects);
	}

	public IEnumerable<object>? GetModelChildren(object model) => (
		(ITreeDataGridSource)treeDataGridSourceImplementation).GetModelChildren(model);
	public bool SortBy(IColumn column, ListSortDirection direction) => ((ITreeDataGridSource)
		treeDataGridSourceImplementation).SortBy(column, direction);

	public ColumnList<TModel> Columns => treeDataGridSourceImplementation.Columns;
	IColumns ITreeDataGridSource.Columns => Columns;

	public IRows Rows => treeDataGridSourceImplementation.Rows;

	public ITreeDataGridSelection? Selection => treeDataGridSourceImplementation.Selection;

	public bool IsHierarchical => treeDataGridSourceImplementation.IsHierarchical;

	public bool IsSorted => treeDataGridSourceImplementation.IsSorted;

	IEnumerable<TModel> ITreeDataGridSource<TModel>.Items => treeDataGridSourceImplementation.Items;

	IEnumerable<object> ITreeDataGridSource.Items => ((ITreeDataGridSource)treeDataGridSourceImplementation).Items;

	public event Action? Sorted {
		add => treeDataGridSourceImplementation.Sorted += value;
		remove => treeDataGridSourceImplementation.Sorted -= value;
	}
}
