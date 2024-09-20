using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Columns;

public class DynamicTemplateColumn<TModel> : FilterableTemplateColumn<TModel> {
    public DynamicTemplateColumn(object? header,
                                 IDataTemplate cellTemplate,
                                 IDataTemplate? cellEditingTemplate = null,
                                 GridLength? width = null,
                                 TemplateColumnOptions<TModel>? options = null) : base(header, cellTemplate,
        cellEditingTemplate, width, options) { }

    public DynamicTemplateColumn(object? header,
                                 object cellTemplateResourceKey,
                                 object? cellEditingTemplateResourceKey = null,
                                 GridLength? width = null,
                                 TemplateColumnOptions<TModel>? options = null) : base(header, cellTemplateResourceKey,
        cellEditingTemplateResourceKey, width, options) { }
}
