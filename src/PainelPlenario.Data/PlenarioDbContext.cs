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

        // Nossas tabelas no PostgreSQL
        public DbSet<Parlamentar> Parlamentares => Set<Parlamentar>();
        public DbSet<Sessao> Sessoes => Set<Sessao>();
        public DbSet<ItemPauta> ItensPauta => Set<ItemPauta>();
        public DbSet<RegistroPresenca> Presencas => Set<RegistroPresenca>();
        public DbSet<RegistroVoto> Votos => Set<RegistroVoto>();

        // Método que cria o banco automaticamente e insere a bancada inicial se estiver vazio
        public async Task InicializarBancoAsync()
        {
            // Cria todas as tabelas no PostgreSQL se ainda não existirem
            await Database.EnsureCreatedAsync();

            // Se a tabela de vereadores estiver vazia, cria os parlamentares padrão
            if (!await Parlamentares.AnyAsync())
            {
                var vereadoresIniciais = new List<Parlamentar>
                {
                    new() { NomeCompleto = "Carlos Eduardo Silva", NomePolitico = "Ver. Carlos Silva", Partido = "MDB", CargoMesa = "Presidente" },
                    new() { NomeCompleto = "Maria de Lourdes Santos", NomePolitico = "Verª. Maria Santos", Partido = "PL", CargoMesa = "1ª Secretária" },
                    new() { NomeCompleto = "José Ribamar dos Santos", NomePolitico = "Ver. Zé da Farmácia", Partido = "UNIÃO", CargoMesa = "Vereador" },
                    new() { NomeCompleto = "Eduardo Castro Alencar", NomePolitico = "Ver. Dr. Eduardo", Partido = "PSD", CargoMesa = "Vereador" },
                    new() { NomeCompleto = "Ana Paula Nogueira", NomePolitico = "Verª. Ana Paula", Partido = "PT", CargoMesa = "Vereadora" },
                    new() { NomeCompleto = "Marcos Paulo Ferreira", NomePolitico = "Ver. Marcos Paulo", Partido = "PP", CargoMesa = "Vereador" },
                    new() { NomeCompleto = "João Batista de Oliveira", NomePolitico = "Ver. Pastor João", Partido = "REPUBLICANOS", CargoMesa = "Vereador" },
                    new() { NomeCompleto = "Lucas Lima Miranda", NomePolitico = "Ver. Lucas Lima", Partido = "PODE", CargoMesa = "Vereador" },
                    new() { NomeCompleto = "Roberto Souza Dias", NomePolitico = "Ver. Roberto Souza", Partido = "PSDB", CargoMesa = "Vereador" },
                };

                await Parlamentares.AddRangeAsync(vereadoresIniciais);
                await SaveChangesAsync();
            }
        }
    }
}