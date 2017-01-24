#region using

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    internal class InstanceCreator
    {
        #region Static Methods

        public static object CreateInstance(Type contextType)
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

        private static bool PrepareMethodParameters(ParameterInfo[] parameters, Type dbContextType, out object[] values)
        {
            if (parameters == null || parameters.Length == 0)
            {
                values = null;
                return true;
            }
            values = new object[] {parameters.Length};
            for (var index = 0; index < parameters.Length; index++)
            {
                var ii = parameters[index];
                if (ii.ParameterType == typeof(DbContextOptions))
                {
                    values[index] = CreateDbContextOptions(dbContextType);
                    continue;
                }
                values = null;
                return false;
            }
            return true;
        }

        #endregion
    }
}