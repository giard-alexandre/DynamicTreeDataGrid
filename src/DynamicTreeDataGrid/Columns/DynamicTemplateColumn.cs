using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Columns;

public class DynamicTemplateColumn<TModel> : TemplateColumn<TModel>, IDynamicColumn<TModel> {
    private bool _visible = true;

    public DynamicTemplateColumn(string name,
                                 object? header,
                                 IDataTemplate cellTemplate,
                                 IDataTemplate? cellEditingTemplate = null,
                                 GridLength? width = null,
                                 TemplateColumnOptions<TModel>? options = null) : base(header, cellTemplate,
        cellEditingTemplate, width, options) {
        Name = name;
    }

    public DynamicTemplateColumn(string name,
                                 object? header,
                                 object cellTemplateResourceKey,
                                 object? cellEditingTemplateResourceKey = null,
                                 GridLength? width = null,
                                 TemplateColumnOptions<TModel>? options = null) : base(header, cellTemplateResourceKey,
        cellEditingTemplateResourceKey, width, options) {
        Name = name;
    }

    public string Name { get; init; }

    public bool Visible {
        get => _visible;
        set {
            if (value == _visible) return;
            _visible = value;
            RaisePropertyChanged();
        }
    }
}
