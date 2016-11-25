#region using

using System;
using System.Collections.Generic;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class DbSetInfo
    {
        #region Constructors

        public DbSetInfo(Type entityType, string tableName)
        {
            EntityType = entityType;
            TableName = tableName;
            Schema = "dbo";
        }

        #endregion

        #region Properties

        public string TableName { get; set; }
        public string Schema { get; set; }
        public Type EntityType { get; private set; }
        public List<ColumnInfo> Properites { get; } = new List<ColumnInfo>();

        #endregion
    }
}