#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class InstanceCreator
    {
        #region Constructors

        public InstanceCreator(IShamanLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Static Methods

        public static T CreateInstance<T>(IShamanLogger logger, params object[] values)
        {
            var inst = new InstanceCreator(logger) { ConstructorParameters = values?.ToList() ?? new List<object>() };
            return (T)inst.CreateInstanceInternal(typeof(T));
        }

        public static object CreateInstance(Type contextType, IShamanLogger logger, params object[] constructorParameters)
        {
            var inst = new InstanceCreator(logger) { ConstructorParameters = constructorParameters?.ToList() ?? new List<object>() };
            return inst.CreateInstanceInternal(contextType);
        }

        #endregion

        #region Instance Methods

        private object CreateDbContextOptions(Type dbContextType)
        {
            var m = typeof(InstanceCreator).GetMethod(nameof(CreateDbContextOptionsInternal),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .MakeGenericMethod(dbContextType);
            var options = m.Invoke(this, null);
            return options;
        }

        private DbContextOptions<T> CreateDbContextOptionsInternal<T>() where T : DbContext
        {
            Log(nameof(CreateDbContextOptionsInternal), $"Create {typeof(DbContextOptions<T>)} with InMemoryDatabase");
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase()
                .Options;
            return options;
        }

        private object CreateInstanceInternal(Type contextType)
        {
            Action<string> log = message => Log(nameof(CreateInstanceInternal), message);
            var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(contextType);
            var constructors =
                contextType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .OrderBy(info => info, new ConstructorInfoComparer(dbContextOptionsType))
                    .ToArray();
            foreach (var constructorInfo in constructors)
            {
                log($"Try constructor {constructorInfo}");
                var parameters = constructorInfo.GetParameters();
                object[] values;
                if (!PrepareMethodParameters(parameters, contextType, out values)) continue;
                log($"Call constructor {constructorInfo}");
                var instance = constructorInfo.Invoke(values);
                return instance;
            }
            log($"Unable to create instance of type {contextType}");
            return null;
        }

        private object CreateParameter(Type parameterType, Type dbContextType)
        {
            foreach (var value in ConstructorParameters)
                if (parameterType.IsInstanceOfType(value))
                {
                    Log(nameof(CreateParameter), $"Using injected value of type {parameterType}");
                    return value;
                }
            var dbCtx = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
            if (parameterType.IsAssignableFrom(dbCtx))
                return CreateDbContextOptions(dbContextType);
            Log(nameof(CreateParameter), $"Unable to create value of type {parameterType}");
            return null;
        }

        private void Log(string methodName, string message)
        {
            _logger.Log(typeof(InstanceCreator), methodName, message);
        }

        private bool PrepareMethodParameters(IReadOnlyList<ParameterInfo> parameters, Type dbContextType,
            out object[] values)
        {
            if (parameters == null || parameters.Count == 0)
            {
                values = null;
                return true;
            }
            values = new object[] { parameters.Count };
            for (var index = 0; index < parameters.Count; index++)
            {
                var ii = parameters[index];
                var parameterValue = CreateParameter(ii.ParameterType, dbContextType);
                if (parameterValue == null)
                {
                    values = null;
                    return false;
                }
                values[index] = parameterValue;
            }
            return true;
        }

        #endregion

        #region Properties

        public List<object> ConstructorParameters { get; private set; } = new List<object>();

        #endregion

        #region Fields

        private readonly IShamanLogger _logger;

        #endregion

        #region Nested

        private class ConstructorInfoComparer : IComparer<ConstructorInfo>
        {
            #region Constructors

            public ConstructorInfoComparer(Type dbContextOptionsType)
            {
                _dbContextOptionsType = dbContextOptionsType;
            }

            #endregion

            #region Instance Methods

            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                var xParameterPriority = GetParameterPriority(x);
                var yParameterPriority = GetParameterPriority(y);
                var compare = xParameterPriority.CompareTo(yParameterPriority);
                if (compare != 0) return -compare; // descending;
                var xGetParametersLength = x.GetParameters().Length;
                var yGetParametersLength = y.GetParameters().Length;
                return xGetParametersLength.CompareTo(yGetParametersLength);
            }

            private int GetParameterPriority(ConstructorInfo constructorInfo)
            {
                var p = constructorInfo.GetParameters();
                var dbOpt = p.Count(i => i.ParameterType.IsAssignableFrom(_dbContextOptionsType));
                var other = p.Length - dbOpt;
                return dbOpt - other;
            }

            #endregion

            #region Fields

            private readonly Type _dbContextOptionsType;

            #endregion
        }

        #endregion
    }
}