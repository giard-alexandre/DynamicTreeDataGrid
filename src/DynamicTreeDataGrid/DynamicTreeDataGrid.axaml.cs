using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DynamicTreeDataGrid;

public partial class DynamicTreeDataGrid : UserControl {
	public static readonly StyledProperty<IDynamicTreeDataGridSource> SourceProperty =
		AvaloniaProperty.Register<DynamicTreeDataGrid, IDynamicTreeDataGridSource>(nameof(Source), defaultValue: null);

	public IDynamicTreeDataGridSource Source {
		get => GetValue(SourceProperty);
		set => SetValue(SourceProperty, value);
	}

	public DynamicTreeDataGrid() { InitializeComponent(); }
}
