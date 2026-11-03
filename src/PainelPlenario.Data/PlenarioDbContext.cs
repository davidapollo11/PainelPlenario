using Microsoft.EntityFrameworkCore;
using PainelPlenario.Core.Enums;
using PainelPlenario.Core.Models;

namespace PainelPlenario.Data
{
    public class PlenarioDbContext : DbContext
    {
        public PlenarioDbContext(DbContextOptions<PlenarioDbContext> options) : base(options)
        {
        }

        // Tabelas no PostgreSQL
        public DbSet<ConfiguracaoCamara> Configuracoes => Set<ConfiguracaoCamara>();
        public DbSet<Parlamentar> Parlamentares => Set<Parlamentar>();
        public DbSet<Sessao> Sessoes => Set<Sessao>();
        public DbSet<ItemPauta> ItensPauta => Set<ItemPauta>();
        public DbSet<RegistroPresenca> Presencas => Set<RegistroPresenca>();
        public DbSet<RegistroVoto> Votos => Set<RegistroVoto>();

        // Método que cria o banco automaticamente e insere a bancada inicial se estiver vazio
        public async Task InicializarBancoAsync()
        {
            // Aplica todas as migrations pendentes automaticamente no PostgreSQL
            await Database.MigrateAsync();

            // 1. Configuração Padrão da Câmara
            if (!await Configuracoes.AnyAsync())
            {
                var configPadrao = new ConfiguracaoCamara
                {
                    Id = 1,
                    NomeCamara = "Câmara Municipal",
                    Slogan = "A Casa do Povo em defesa da cidadania",
                    Cidade = "Município",
                    Estado = "UF",
                    Legislatura = "Legislatura 2025-2028"
                };

                await Configuracoes.AddAsync(configPadrao);
                await SaveChangesAsync();
            }
        }
    }
}