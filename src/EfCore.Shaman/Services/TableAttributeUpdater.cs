#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class TableAttributeUpdater : IDbSetInfoUpdateService
    {
        #region Instance Methods

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType)
        {
            var ta = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (ta == null) return;
            dbSetInfo.TableName = ta.Name;
            if (!string.IsNullOrEmpty(ta.Schema))
                dbSetInfo.Schema = ta.Schema;
        }

        #endregion
    }
}