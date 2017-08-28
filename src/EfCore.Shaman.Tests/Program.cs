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
            new ModelInfoTests().T06_ShouldHaveTableNameWithPrefix();
            new TimestampAttributeUpdaterTest().T01_ShouldMarkColumnAsTimestamp();
        }
    }
    
}
