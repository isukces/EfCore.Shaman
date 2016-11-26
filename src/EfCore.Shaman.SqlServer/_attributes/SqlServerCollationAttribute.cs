using System;
using System.Xml.Schema;

namespace EfCore.Shaman.SqlServer
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SqlServerCollationAttribute : Attribute
    {
        public string Collation { get; set; }
    }
}
