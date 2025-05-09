using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CashFlow.Data;
using CashFlow.ViewModels;
using CashFlow.Views;
using System.Windows;
using CashFlow.Services;
using CashFlow.Core;
using CashFlow.Interfaces;
using CashFlow.Repositories;
using CashFlow.Uow;

namespace CashFlow;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    public App()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddDbContextFactory<CashFlowContext>(options =>
            options.UseSqlite("Data Source=cashflow.db")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .EnableSensitiveDataLogging(),
            ServiceLifetime.Scoped);

        // Registra i repository
        services.AddRepositories();
        // Registra i viewModel
        services.AddViewModels();
        // Registra i servizi
        services.AddServices();
        // Registra le finestre
        services.AddViews();

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

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // Registrazione dei repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITransactionRepository, TransactionRepository>();
    }

    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<NavigationViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<SummaryViewModel>();
        services.AddTransient<ActivityViewModel>();
    }
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<NavigationService>();
        services.AddSingleton<ViewModelFactory>();
    }

    public static void AddViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<NavigationViewModel>()
        });
        services.AddTransient<TransactionView>();
        services.AddTransient<SummaryView>();
        services.AddTransient<ActivityView>();
    }
}