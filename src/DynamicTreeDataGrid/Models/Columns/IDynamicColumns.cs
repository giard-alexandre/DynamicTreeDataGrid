using System.Collections.Specialized;

namespace DynamicTreeDataGrid.Models.Columns;

public interface IDynamicColumns : IReadOnlyList<IDynamicColumn>, INotifyCollectionChanged;
