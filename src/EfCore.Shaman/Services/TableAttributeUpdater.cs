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

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger)
        {
            var ta = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (ta == null) return;
            dbSetInfo.TableName = ta.Name;
            if (!string.IsNullOrEmpty(ta.Schema))
                dbSetInfo.Schema = ta.Schema;
            logger.Log(typeof(TableAttributeUpdater), nameof(UpdateDbSetInfo),
                $"Set Schema='{dbSetInfo.Schema}' for table '{dbSetInfo.TableName}'");
        }

        #endregion
    }
}