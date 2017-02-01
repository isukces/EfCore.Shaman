#region using

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.SqlServer
{
    /// <summary>
    ///     Provides DbContextOptions with Sql server support.
    ///     Fake connection string is used. 
    ///     Shaman never uses created dbcontext for storing or reading data, so connectionstring doesnt matter.
    /// </summary>
    public class SqlDbContextValueProvider : IValueProviderService
    {
        #region Instance Methods

        public IEnumerable<object> CreateObjects()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer("Initial Catalog=Fake_shaman_database")
                .Options;
            yield return options;
        }

        #endregion
    }
}