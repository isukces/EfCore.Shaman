using System.ComponentModel.DataAnnotations;

namespace EfCore.Shaman.Tests.Model
{
    public class User
    {
        #region Properties
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        #endregion
    }

    public class UserWithTimestamp
    {
        #region Properties
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        #endregion
    }
}