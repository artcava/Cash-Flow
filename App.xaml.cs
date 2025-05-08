using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CashFlow.Data;
using CashFlow.ViewModels;
using CashFlow.Views;
using System.Windows;
using CashFlow.Services;
using CashFlow.Core;

namespace CashFlow;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    public App()
    {
        IServiceCollection services = new ServiceCollection();

        // Configura SQLite
        services.AddDbContext<CashFlowContext>(options =>
            options.UseSqlite("Data Source=cashflow.db")
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        // Registra i viewModel
        services.AddSingleton<NavigationViewModel>();
        services.AddSingleton<TransactionViewModel>();
        services.AddSingleton<SummaryViewModel>();
        services.AddSingleton<ActivityViewModel>();
        // Registra i servizi
        services.AddSingleton<NavigationService>();
        services.AddSingleton<ViewModelFactory>();
        // Registra le finestre
        services.AddSingleton<MainWindow>(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<NavigationViewModel>()
        });

        services.AddSingleton<TransactionView>();
        services.AddSingleton<SummaryView>();
        services.AddSingleton<ActivityView>();

        services.AddSingleton<Func<Type, object, ViewModel>>(provider =>
            provider.GetRequiredService<ViewModelFactory>().CreateViewModel
        );

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show(); // Display the main window
        base.OnStartup(e);

        // Inizializza il database
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CashFlowContext>();
            context.Database.EnsureCreated();
        }
    }
}