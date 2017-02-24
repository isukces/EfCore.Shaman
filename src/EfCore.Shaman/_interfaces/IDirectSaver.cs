#region using

using System;
using System.Collections.Generic;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman
{
    public interface IDirectSaver<in T>
    {
        #region Instance Methods

        void Delete(DbContext context, IReadOnlyDictionary<string, object> values);
        IReadOnlyList<ColumnInfo> GetPrimaryKeyColumns();
        void Insert(DbContext context, T entity);
        void Update(DbContext context, T entity);

        #endregion
    }

    public static class DirectSaverExtensions
    {
        #region Static Methods

        public static void Delete<T>(this IDirectSaver<T> saver, DbContext context, T entity)
        {
            var pk = saver.GetPrimaryKeyColumns();
            var dict = new Dictionary<string, object>(pk.Count);
            foreach (var i in pk)
            {
                var value = i.ValueReader.ReadPropertyValue(entity);
                dict[i.ColumnName] = value;
            }
            saver.Delete(context, dict);
        }


        public static void Save<T>(this IDirectSaver<T> saver, DbContext context, T entity,
            DirectSaverEntityStatus status)
        {
            switch (status)
            {
                case DirectSaverEntityStatus.Clean:
                    return;
                case DirectSaverEntityStatus.MustBeInserted:
                    saver.Insert(context, entity);
                    break;
                case DirectSaverEntityStatus.MustBeUpdated:
                    saver.Update(context, entity);
                    break;
                case DirectSaverEntityStatus.MustBeRemoved:
                    saver.Delete(context, entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static void Save<T>(this IDirectSaver<T> saver, DbContext context,
            EntityWithDirectSaverStatus<T> entityWithStatus)
        {
            saver.Save(context, entityWithStatus.Item, entityWithStatus.Status);
        }

        #endregion
    }
}