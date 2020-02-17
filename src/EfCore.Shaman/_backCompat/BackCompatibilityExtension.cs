using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCore.Shaman
{
    public static class BackCompatibilityExtension
    {
        public static RelationalCompatibility Relational(this IEntityType x)
        {
            return new RelationalCompatibility(x);
        }
    }
}