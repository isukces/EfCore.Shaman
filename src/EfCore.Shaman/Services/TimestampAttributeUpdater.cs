using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    internal class TimestampAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<TimestampAttribute>() == null) return;
            columnInfo.IsTimestamp = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(UpdateColumnInfo),
                $"{columnInfo.ColumnName}.IsTimestamp=true");
            columnInfo.IsDatabaseGenerated = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(UpdateColumnInfo),
                $"{columnInfo.ColumnName}.IsDatabaseGenerated=true");
        }

        #endregion
    }
}