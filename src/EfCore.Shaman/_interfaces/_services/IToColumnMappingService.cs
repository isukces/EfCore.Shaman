using System.Reflection;

namespace EfCore.Shaman
{
    /// <summary>
    /// Check if class property is mapped into table column
    /// </summary>
    public interface IToColumnMappingService : IShamanService
    {
        bool IsMappedToTableColumn(PropertyInfo propertyInfo);
    }
}