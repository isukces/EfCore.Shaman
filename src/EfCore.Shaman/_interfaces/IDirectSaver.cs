#region using

using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman
{
    public interface IDirectSaver<in T>
    {
        #region Instance Methods

        void Insert(DbContext context, T entity);
        void Update(DbContext context, T entity);
        void Delete(DbContext context, T entity);

        #endregion
    }
}