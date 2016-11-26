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

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType)
        {
            var ta = entityType.GetCustomAttribute<TableAttribute>();
            if (ta == null) return;
            dbSetInfo.TableName = ta.Name;
            dbSetInfo.Schema = ta.Schema;
        }

        #endregion
    }
}