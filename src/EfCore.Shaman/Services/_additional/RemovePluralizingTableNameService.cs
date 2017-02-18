#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    public class RemovePluralizingTableNameService : IDbSetInfoUpdateService
    {
        #region Instance Methods

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger)
        {
            var tableAttribute = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (tableAttribute == null)
                dbSetInfo.TableName = entityType.Name;
            // todo log
        }

        #endregion
    }
}