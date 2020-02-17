﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman
{
    public interface IDirectSaver<in T>
    {
        void Delete(DbContext context, IReadOnlyDictionary<string, object> values);
        IReadOnlyList<ColumnInfo> GetPrimaryKeyColumns();
        Task InsertAsync(DbContext context, T entity, bool skipSelect = false);
        Task UpdateAsync(DbContext context, T entity, bool skipSelect = false);
    }

    public static class DirectSaverExtensions
    {
        public static void DirectDelete<T>(this IDirectSaver<T> saver, DbContext context, T entity) where T : class
        {
            if (context is IInMemoryDatabaseAwareDbProvider p && p.IsUsingInMemoryDatabase)
                context.Remove(entity);
            else
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
        }

        public static void DirectDelete<T>(this ShamanDbContext context, T item) where T : class
        {
            var ds = context.GetDirectSaver<T>();
            ds.DirectDelete(context, item);
        }


        public static async Task DirectInsertAsync<T>(this ShamanDbContext context, T obj, bool skipSelect = false) where T : class
        {
            if (context.IsUsingInMemoryDatabase)
            {
                context.Add(obj);
                await context.SaveChangesAsync();
            }
            else
            {
                var ds = context.GetDirectSaver<T>();
                await ds.InsertAsync(context, obj, skipSelect);
            }
        }


        public static async Task DirectSaveAsync<T>(this IDirectSaver<T> saver, DbContext context, T entity, DirectSaverEntityStatus status, bool skipSelect = false) where T : class
        {
            if (context is IInMemoryDatabaseAwareDbProvider p && p.IsUsingInMemoryDatabase)
            {
                switch (status)
                {
                    case DirectSaverEntityStatus.Clean:
                        return;
                    case DirectSaverEntityStatus.MustBeInserted:
                        context.Add(entity);
                        context.SaveChanges();
                        break;
                    case DirectSaverEntityStatus.MustBeUpdated:
                        context.SaveChanges();
                        break;
                    case DirectSaverEntityStatus.MustBeRemoved:
                        context.Remove(entity);
                        context.SaveChanges();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
            }
            else
            {
                switch (status)
                {
                    case DirectSaverEntityStatus.Clean:
                        return;
                    case DirectSaverEntityStatus.MustBeInserted:
                        await saver.InsertAsync(context, entity, skipSelect);
                        break;
                    case DirectSaverEntityStatus.MustBeUpdated:
                        await saver.UpdateAsync(context, entity, skipSelect);
                        break;
                    case DirectSaverEntityStatus.MustBeRemoved:
                        saver.DirectDelete(context, entity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
            }
        }

        public static Task DirectSaveAsync<T>(this IDirectSaver<T> saver, DbContext context, EntityWithDirectSaverStatus<T> entityWithStatus, bool skipSelect = false) where T : class
        {
            return saver.DirectSaveAsync(context, entityWithStatus.Item, entityWithStatus.Status, skipSelect);
        }


        public static Task DirectSave<T>(this ShamanDbContext context, EntityWithDirectSaverStatus<T> item, bool skipSelect = false) where T : class
        {
            if (item.Status == DirectSaverEntityStatus.Clean)
                return Task.CompletedTask;
            var ds = context.GetDirectSaver<T>();
            return ds.DirectSaveAsync(context, item, skipSelect);
        }


        public static Task DirectSaveAsync<T>(this ShamanDbContext context, T obj, DirectSaverEntityStatus status, bool skipSelect = false) where T : class
        {
            var ds = context.GetDirectSaver<T>();
            return ds.DirectSaveAsync(context, obj, status, skipSelect);
        }

        public static Task DirectUpdateAsync<T>(this ShamanDbContext context, T obj, bool skipSelect = false)
        {
            var ds = context.GetDirectSaver<T>();
            return ds.UpdateAsync(context, obj, skipSelect);
        }
    }
}