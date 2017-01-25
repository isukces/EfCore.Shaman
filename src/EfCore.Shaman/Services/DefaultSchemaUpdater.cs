#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#endregion

namespace EfCore.Shaman.Services
{
    internal static class DefaultSchemaUpdater
    {
        #region Static Methods

        public static string GetDefaultSchema(Type type, IModel model)
        {
            var schema = type.GetCustomAttribute<DefaultSchemaAttribute>()?.Schema?.Trim();
            if (!string.IsNullOrEmpty(schema))
                return schema;
            schema = model?.Relational().DefaultSchema?.Trim();
            return schema;
        }

        #endregion


    }
}