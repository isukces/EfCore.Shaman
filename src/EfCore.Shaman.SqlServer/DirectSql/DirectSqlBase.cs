#region using

using System.Collections.Generic;
using System.Text;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class DirectSqlBase
    {
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

        protected ColumnInfo[] SqlColumns;

        protected object Entity;

        #endregion
    }
}