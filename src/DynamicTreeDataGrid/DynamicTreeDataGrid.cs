using Avalonia.Controls;

namespace DynamicTreeDataGrid;

public class DynamicTreeDataGrid : TreeDataGrid {

	protected override Type StyleKeyOverride => typeof(TreeDataGrid);

	public DynamicTreeDataGrid() : base() {
	}

	public new ITreeDataGridSource? Source {
		get => base.Source;
		set {
			base.Source = value;
			Console.WriteLine("Test");
		}
	}

}
