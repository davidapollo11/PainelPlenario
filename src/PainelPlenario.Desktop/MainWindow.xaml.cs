using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PainelPlenario.Core.Services;
using PainelPlenario.Data;

namespace PainelPlenario.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        // Permite trabalhar com datas normais (DateTime.Now/Today) no PostgreSQL
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        try
        {
            var services = new ServiceCollection();

            // 1. Blazor WebView
            services.AddWpfBlazorWebView();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            // 2. Conexão com o PostgreSQL do Docker (Factory recomendada para Blazor Hybrid)
            var connectionString = "Host=localhost;Port=5433;Database=plenario_db;Username=postgres;Password=postgres;";
            services.AddDbContextFactory<PlenarioDbContext>(options =>
                options.UseNpgsql(connectionString));

            // 3. Nosso motor de estado
            services.AddSingleton<PlenarioStateService>();

            var serviceProvider = services.BuildServiceProvider();

            // 4. Executa a criação no banco em segundo plano SEM travar a UI Thread!
            Task.Run(async () =>
            {
                var dbFactory = serviceProvider.GetRequiredService<IDbContextFactory<PlenarioDbContext>>();
                await using var db = await dbFactory.CreateDbContextAsync();
                await db.InicializarBancoAsync();
            }).Wait();

            // Registra os serviços para a janela
            Resources.Add("services", serviceProvider);

            InitializeComponent();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro na Inicialização");
        }
    }
}