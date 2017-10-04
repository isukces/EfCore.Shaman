using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman
{
    public interface IColumnInfoUpdateService : IShamanService
    {
        void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger);
    }
}