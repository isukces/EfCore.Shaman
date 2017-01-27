#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal static class DefaultSchemaUpdater
    {
        #region Static Methods

        public static string GetDefaultSchema(Type type, EfModelWrapper model)
        {
            var schema = type.GetTypeInfo().GetCustomAttribute<DefaultSchemaAttribute>()?.Schema?.Trim();
            if (!string.IsNullOrEmpty(schema))
                return schema;
            schema = model.DefaultSchema;
            return schema;
        }

        #endregion


    }
}