#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    public class PrefixedTableNameService : IDbSetInfoUpdateService
    {
        private readonly bool _ignoreTableAttribute;

        #region Constructors

        public PrefixedTableNameService(string prefix, bool ignoreTableAttribute = false)
        {
            _ignoreTableAttribute = ignoreTableAttribute;
            _prefix = prefix?.Trim() ?? "";
        }

        #endregion

        #region Instance Methods

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType)
        {
            var tableAttribute = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (tableAttribute == null || _ignoreTableAttribute)
                dbSetInfo.TableName = _prefix + dbSetInfo.TableName;
        }

        #endregion

        #region Fields

        private readonly string _prefix;

        #endregion
    }
}