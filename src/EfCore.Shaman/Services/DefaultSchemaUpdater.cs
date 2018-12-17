#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal static class DefaultSchemaUpdater
    {
        public static bool GetDefaultIsUnicodeText(Type dbContextType, IShamanLogger logger)
        {
            var fromAttribute = dbContextType.GetTypeInfo().GetCustomAttribute<UnicodeTextAttribute>()?.IsUnicode;
            if (fromAttribute != null)
            {
                logger.Log(typeof(DefaultSchemaUpdater), nameof(GetDefaultIsUnicodeText),
                    "Default UnicodeText from " + nameof(UnicodeTextAttribute) + ": " + fromAttribute);
                return fromAttribute.Value;
            }

            logger.Log(typeof(DefaultSchemaUpdater), nameof(GetDefaultIsUnicodeText),
                "Default UnicodeText = true");
            return true;
        }

        public static string GetDefaultSchema(Type type, EfModelWrapper model, IShamanLogger logger)
        {
            var schema = type.GetTypeInfo().GetCustomAttribute<DefaultSchemaAttribute>()?.Schema?.Trim();
            if (!string.IsNullOrEmpty(schema))
            {
                logger.Log(typeof(DefaultSchemaUpdater), nameof(GetDefaultSchema),
                    "Default schema from " + nameof(DefaultSchemaAttribute) + ": " + schema);
                return schema;
            }

            schema = model.DefaultSchema;
            return schema;
        }
    }
}