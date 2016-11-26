#region using

using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman
{
    public interface IColumnInfoUpdateService : IShamanService
    {
        #region Instance Methods

        void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo);

        #endregion
    }
}