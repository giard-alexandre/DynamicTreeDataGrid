using Avalonia.Controls;

namespace DynamicExtensions.TreeDataGrid;

public class DynamicTreeDataGrid : Avalonia.Controls.TreeDataGrid {

	protected override Type StyleKeyOverride => typeof(Avalonia.Controls.TreeDataGrid);

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
