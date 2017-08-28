using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.Tests.EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCore.Shaman.Tests
{
    
    public class Program
    {
        public static void Main()
        {
            IEntityType t = null;
            new TimestampAttributeUpdaterTest().T01_ShouldMarkColumnAsTimestamp();
        }
    }
    
}
