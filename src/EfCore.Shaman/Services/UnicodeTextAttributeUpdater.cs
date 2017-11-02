using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    internal class UnicodeTextAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<UnicodeTextAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(UnicodeTextAttribute), nameof(UpdateColumnInfo),
                $"Set IsUnicode={indexAttribute.IsUnicode}");
            columnInfo.IsUnicode = indexAttribute.IsUnicode;
        }
    }
}