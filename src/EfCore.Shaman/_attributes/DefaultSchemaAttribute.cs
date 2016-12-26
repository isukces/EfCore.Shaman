using System;

namespace EfCore.Shaman
{
    /// <summary>
    /// Decorates Context class in order to set default schema for all tables.
    /// Use this intead of modelBuilder.HasDefaultSchema
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultSchemaAttribute : Attribute
    {
        public DefaultSchemaAttribute(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; set; }
    }
}
