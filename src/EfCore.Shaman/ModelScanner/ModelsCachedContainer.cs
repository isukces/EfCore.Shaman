#region using

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class ModelsCachedContainer
    {
        #region Static�Methods

        public static EfModelWrapper GetRawModel(Type dbContextType)
        {
            return Cache.GetOrAdd(dbContextType, t =>
            {
                var model = GetModel(t, false);
                return model;
            });
        }


        public static void SetRawModel(Type type, IMutableModel modelBuilderModel)
        {
            var value = EfModelWrapper.FromModel(modelBuilderModel);
            Cache.TryAdd(type, value);
        }

        private static EfModelWrapper GetModel(Type dbContextType, bool raw)
        {
            try
            {
                var sandboxed = new ModelsCachedContainer
                {
                    DbContextType = dbContextType,
                    Raw = raw
                };
                return sandboxed.GetModel();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }


        #endregion

        #region Instance Methods

        private EfModelWrapper GetModel()
        {
            var instance = TryCreateInstance(DbContextType);
            if (instance == null) return null;
            var model = EfModelWrapper.FromModel(instance.Model);
            return model;
        }

        private DbContext TryCreateInstance(Type contextType)
        {
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));
            var method = contextType.FindStaticMethodWihoutParameters("GetDbContext");
            if (method != null && contextType.IsAssignableFrom(method.ReturnType))
                return (DbContext)method.Invoke(null, null);
            return InstanceCreator.CreateInstance(contextType) as DbContext;
        }

        #endregion

        #region Properties

        private bool Raw { get; set; }

        private Type DbContextType { get; set; }

        #endregion

        #region Static Fields

        private static readonly ConcurrentDictionary<Type, EfModelWrapper> Cache =
            new ConcurrentDictionary<Type, EfModelWrapper>();

        #endregion
    }
}