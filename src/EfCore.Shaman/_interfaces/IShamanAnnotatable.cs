using System.Collections.Generic;

namespace EfCore.Shaman
{
    public interface IShamanAnnotatable
    {
        IDictionary<string, object> Annotations { get; }
    }

    public static class ShamanAnnotatableExtensions
    {
        public static string GetStringAnnotation(this IShamanAnnotatable annotatable, string key)
        {
            if (annotatable == null)
                return null;
            if (annotatable.Annotations.TryGetValue(key, out var value))
                return value as string;
            return null;
        }
    }
}