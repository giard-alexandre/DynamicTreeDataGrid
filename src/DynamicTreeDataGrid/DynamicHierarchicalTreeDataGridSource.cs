using System.ComponentModel;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Input;

namespace DynamicTreeDataGrid;

// TODO: Implement this for real :)
public class DynamicHierarchicalTreeDataGridSource<TModel> : IDynamicTreeDataGridSource<TModel>
	where TModel : class {
	private ITreeDataGridSource<TModel> treeDataGridSourceImplementation;
	public event PropertyChangedEventHandler? PropertyChanged {
		add => treeDataGridSourceImplementation.PropertyChanged += value;
		remove => treeDataGridSourceImplementation.PropertyChanged -= value;
	}

	public void DragDropRows(ITreeDataGridSource source,
	                         IEnumerable<IndexPath> indexes,
	                         IndexPath targetIndex,
	                         TreeDataGridRowDropPosition position,
	                         DragDropEffects effects) {
		treeDataGridSourceImplementation.DragDropRows(source, indexes, targetIndex, position, effects);
	}

	public IEnumerable<object>? GetModelChildren(object model) => treeDataGridSourceImplementation.GetModelChildren(model);
	public bool SortBy(IColumn column, ListSortDirection direction) => treeDataGridSourceImplementation.SortBy(column, direction);

	public IColumns Columns => treeDataGridSourceImplementation.Columns;

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
