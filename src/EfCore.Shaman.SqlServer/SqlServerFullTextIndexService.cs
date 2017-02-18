using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCore.Shaman.SqlServer
{
    internal class SqlServerFullTextIndexService : IFixMigrationUpService
    {
        public void FixMigrationUp(MigrationBuilder migrationBuilder, ModelInfo info)
        {
           // throw new NotImplementedException();
        }
    }
}
