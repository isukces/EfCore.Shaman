#region using

using System;
using System.Collections.Generic;
using System.Text;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class DirectSqlBase
    {
        #region Constructors

        protected DirectSqlBase()
        {
            _pkColumns = new Lazy<IReadOnlyList<ColumnInfo>>(GetPkColumnsInternal);
        }

        #endregion

        #region Static Methods

        public static string Encode(string x)
        {
            return "[" + x + "]";
        }

        #endregion

        #region Instance Methods

        protected void AddSeparator(string newSeparator)
        {
            if (Separator == null)
                Separator = newSeparator;
            else
                SqlText.Append(Separator);
        }

        protected IReadOnlyList<ColumnInfo> PkColumns => _pkColumns.Value;

        private IReadOnlyList<ColumnInfo> GetPkColumnsInternal()
        {
            var columnInfos = new List<ColumnInfo>(SqlColumns.Count);
            foreach (var i in SqlColumns)
                if (i.IsInPrimaryKey)
                    columnInfos.Add(i);
            return columnInfos.ToArray();
        }

        #endregion

        #region Properties

        protected string TableName => Encode(FullTableName.Schema) + "." + Encode(FullTableName.TableName);

        #endregion

        #region Fields

        protected readonly StringBuilder SqlText = new StringBuilder();

        protected readonly List<object> ParameterValues = new List<object>();
        protected string Separator;

        protected IFullTableName FullTableName;

        protected ColumnInfo IdentityColumn;

        protected IReadOnlyList<ColumnInfo> SqlColumns;

        private readonly Lazy<IReadOnlyList<ColumnInfo>> _pkColumns;

        protected object Entity;

        #endregion
    }
}