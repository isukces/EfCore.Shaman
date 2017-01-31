#region using

using System;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class SqlServerDirectSaver<T> : IDirectSaver<T>
    {
        #region Constructors

        private SqlServerDirectSaver(DbSetInfo dbSetInfo)
        {
            _info = dbSetInfo;
            _identityColumn = _info.Properites.SingleOrDefault(a => a.IsIdentity);
            _sqlColumns = _info.Properites.Where(a => !a.IsNotMapped && !a.IsNavigationProperty).ToArray();
        }

        #endregion

        #region Static Methods

        public static SqlServerDirectSaver<T> FromContext(Type dbContextType, Func<ShamanOptions> optionsFactory = null)
        {
            if (optionsFactory == null)
                optionsFactory = () => new ShamanOptions().WithDefaultServices().WithSqlServer();
            var options = optionsFactory();
            var info = new ModelInfo(dbContextType, options);
            var dbSetInfo = info.DbSets.FirstOrDefault(a => a.EntityType == typeof(T));
            if (dbSetInfo == null)
                throw new Exception("Context doesn't contain entity type " + typeof(T));
            return new SqlServerDirectSaver<T>(dbSetInfo);
        }

        #endregion

        #region Instance Methods

        public void Insert(DbContext context, T entity)
        {
            InsertBuilder.DoInsert(_info, context, _sqlColumns, _identityColumn, entity);
        }

        public void Update(DbContext context, T entity)
        {
            UpdateBuilder.DoUpdate(_info, context, _sqlColumns, _identityColumn, entity);
        }

        #endregion

        #region Fields

        private readonly DbSetInfo _info;
        private readonly ColumnInfo _identityColumn;
        private readonly ColumnInfo[] _sqlColumns;

        #endregion
    }
}