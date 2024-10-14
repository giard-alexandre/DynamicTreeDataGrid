using DynamicTreeDataGrid.State;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumns : IDynamicColumnsBase {
	IDynamicColumnsBase DisplayedColumns { get; }
	void Move(int oldIndex, int newIndex);
	void Move(IDynamicColumn oldLocation, IDynamicColumn newLocation);

	/// <summary>
	/// Apply the supplied column state. Applies column indexes and visibility.
	/// </summary>
	/// <param name="states">The states to apply</param>
	/// <returns><c>true</c> if applied successfully, <c>false</c> if any errors occurred.</returns>
	bool ApplyColumnStates(IEnumerable<ColumnState> states);

	/// <summary>
	/// Get the current <see cref="ColumnState"/> list of the collection.
	/// </summary>
	/// <returns>The current collection of <see cref="ColumnState"/>s</returns>
	IList<ColumnState> GetColumnStates();

}
