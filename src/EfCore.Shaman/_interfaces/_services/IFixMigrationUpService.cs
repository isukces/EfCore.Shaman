using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCore.Shaman
{
    public interface IFixMigrationUpService : IShamanService
    {
        void FixMigrationUp(MigrationBuilder migrationBuilder, ModelInfo info);
    }
}