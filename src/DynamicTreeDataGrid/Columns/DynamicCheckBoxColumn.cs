using System.Linq.Expressions;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicTreeDataGrid.Models.Columns;

namespace DynamicTreeDataGrid.Columns;

/// <summary>
/// A column in an <see cref="ITreeDataGridSource"/> which displays a check box.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <remarks>
/// Extra abstraction over <seealso cref="FilterableCheckBoxColumn{TModel}"/> as that class is meant to copy
/// <seealso cref="CheckBoxColumn{TModel}"/> as closely as possible. This class adds all of this packages
/// extra features to it!
/// </remarks>
public class DynamicCheckBoxColumn<TModel> : FilterableCheckBoxColumn<TModel>
    where TModel : class {
    public DynamicCheckBoxColumn(object? header,
                                 Expression<Func<TModel, bool>> getter,
                                 Action<TModel, bool>? setter = null,
                                 GridLength? width = null,
                                 CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
        options) { }

    public DynamicCheckBoxColumn(object? header,
                                 Expression<Func<TModel, bool?>> getter,
                                 Action<TModel, bool?>? setter = null,
                                 GridLength? width = null,
                                 CheckBoxColumnOptions<TModel>? options = null) : base(header, getter, setter, width,
        options) { }
}
