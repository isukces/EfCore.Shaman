#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class ModelsCachedContainer
    {
        #region Instance Methods

        private EfModelWrapper GetModelInternal()
        {
            var instance = TryCreateInstance(DbContextType);
            if (instance == null) return null;
            var model = EfModelWrapper.FromModel(instance.Model);
            return model;
        }

        private DbContext TryCreateInstance(Type contextType)
        {
            Action<string> log = message =>
                     Logger.Log(typeof(ModelsCachedContainer), nameof(TryCreateInstance), message);
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));
            var method = contextType.FindStaticMethodWihoutParameters("GetDbContext");
            if (method != null)
            {
                var methodName = $"{method.DeclaringType?.Name}.{method.Name}".TrimStart('.');
                log($"Found method {methodName}");
                if (contextType.IsAssignableFrom(method.ReturnType))
                {
                    log($"Call method {methodName}");
                    return (DbContext)method.Invoke(null, null);
                }
                log(
                    $"Skip calling method {methodName} because of return type {method.ReturnType} instead of {contextType}.");
            }
            else
                log($"Method GetDbContext not found in type {contextType}");
            var services = ShamanOptions.CreateShamanOptions(contextType).Services.OfType<IValueProviderService>();
            var constructorParameters = services.SelectMany(a => a.CreateObjects()).ToArray();
            return InstanceCreator.CreateInstance(contextType, Logger, constructorParameters) as DbContext;
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

        #region Static Methods

        public static EfModelWrapper GetRawModel(Type dbContextType, IShamanLogger logger)
        {
            return Cache.GetOrAdd(dbContextType, t =>
            {
                var model = GetModel(t, false, logger);
                return model;
            });
        }


        public static void SetRawModel(Type type, IMutableModel model, IShamanLogger logger)
        {
            Action<string> log = message => logger.Log(typeof(ModelsCachedContainer), nameof(SetRawModel), message);
            var tables = from i in model.GetEntityTypes()
                         let r = i.Relational()
                         select $"{r.Schema}.{r.TableName}";
            log($"Try set model containing tables: {string.Join(",", tables)}");
            var value = EfModelWrapper.FromModel(model);
            var result = Cache.TryAdd(type, value);
            log(result ? "Success" : "Skipped");
        }

        private static EfModelWrapper GetModel(Type dbContextType, bool raw, IShamanLogger logger)
        {
            Action<string> log = message => logger.Log(typeof(ModelsCachedContainer), nameof(GetModelInternal), message);
            try
            {
                var instance = new ModelsCachedContainer
                {
                    DbContextType = dbContextType,
                    Raw = raw,
                    Logger = logger
                };
                return instance.GetModelInternal();
            }
            catch (Exception e)
            {
                log("Exception " + e.Message);
                throw;
            }
        }

        public IShamanLogger Logger { get; set; } = EmptyShamanLogger.Instance;

        #endregion
    }
}