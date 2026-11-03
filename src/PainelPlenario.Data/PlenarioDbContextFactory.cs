using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PainelPlenario.Data;

public class PlenarioDbContextFactory : IDesignTimeDbContextFactory<PlenarioDbContext>
{
    public PlenarioDbContext CreateDbContext(string[] args)
    {
        // Garante suporte a datas normais no PostgreSQL em tempo de design
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var optionsBuilder = new DbContextOptionsBuilder<PlenarioDbContext>();

        // A conexão com o container do Docker
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=plenario_db;Username=postgres;Password=postgres;");

        return new PlenarioDbContext(optionsBuilder.Options);
    }
}