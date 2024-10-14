using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace DynamicTreeDataGrid.Filtering;

public class FilterDropDownContentSelector : IDataTemplate {
    // This Dictionary should store our shapes. We mark this as [Content], so we can directly add elements to it later.
    [Content] public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new();

    // Build the DataTemplate here
    public Control Build(object? param) {
        if (param is null) throw new ArgumentNullException(nameof(param));

        return new Arc();


        // return AvailableTemplates[key].Build(param); // finally we look up the provided key and let the System build the DataTemplate for us
    }

    // Check if we can accept the provided data
    public bool Match(object? data) {
        if (data is null) return false;
        var dataType = data.GetType();
        dataType = Nullable.GetUnderlyingType(dataType) ?? dataType;

        return dataType.IsEnum || dataType.IsPrimitive;
    }

    private static bool IsNumericType(object? value) {
        if (value is null) return false;

        var type = value.GetType();
        var typeCode = Type.GetTypeCode(type);

        switch (typeCode) {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                return true;
            default:
                return false;
        }
    }
}
