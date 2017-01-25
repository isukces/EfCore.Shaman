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
        #region Static Methods

        public static T CreateInstance<T>(params object[] values)
        {
            var inst = new InstanceCreator {Values = values?.ToList() ?? new List<object>()};
            return (T)inst.CreateInstanceInternal(typeof(T));
        }

        public static object CreateInstance(Type contextType, params object[] values)
        {
            var inst = new InstanceCreator {Values = values?.ToList() ?? new List<object>()};
            return inst.CreateInstanceInternal(contextType);
        }

        private static object CreateDbContextOptions(Type dbContextType)
        {
            var m = typeof(InstanceCreator).GetMethod(nameof(CreateDbContextOptionsInternal),
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .MakeGenericMethod(dbContextType);
            var options = m.Invoke(null, null);
            return options;
        }

        private static DbContextOptions<T> CreateDbContextOptionsInternal<T>() where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase()
                .Options;
            return options;
        }

        #endregion

        #region Instance Methods

        private object CreateInstanceInternal(Type contextType)
        {
            var constructors =
                contextType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .OrderBy(a => a.GetParameters().Length)
                    .ToArray();
            foreach (var constructorInfo in constructors)
            {
                var parameters = constructorInfo.GetParameters();
                object[] values;
                if (!PrepareMethodParameters(parameters, contextType, out values)) continue;
                var instance = constructorInfo.Invoke(values);
                return instance;
            }
            return null;
        }

        private object CreateParameter(Type parameterType, Type dbContextType)
        {
            foreach (var i in Values)
                if (parameterType.IsInstanceOfType(i))
                    return i;
            var dbCtx = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
            if (parameterType.IsAssignableFrom(dbCtx))
                return CreateDbContextOptions(dbContextType);
            return null;
        }

        private bool PrepareMethodParameters(IReadOnlyList<ParameterInfo> parameters, Type dbContextType,
            out object[] values)
        {
            if (parameters == null || parameters.Count == 0)
            {
                values = null;
                return true;
            }
            values = new object[] {parameters.Count};
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

        public List<object> Values { get; private set; } = new List<object>();

        #endregion
    }
}