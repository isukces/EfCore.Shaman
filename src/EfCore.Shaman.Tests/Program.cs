using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.Tests.EfCore.Shaman.Services;

namespace EfCore.Shaman.Tests
{
    public class Program
    {
        public static void Main()
        {
            new TimestampAttributeUpdaterTest().T01_ShouldMarkColumnAsTimestamp();
        }
    }
}
