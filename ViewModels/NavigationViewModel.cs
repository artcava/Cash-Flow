using CashFlow.Core;
using CashFlow.Services;
using System.ComponentModel;

namespace CashFlow.ViewModels;

public class NavigationViewModel : ViewModel
{
    private readonly NavigationService _navigationService;

    public ViewModel CurrentView => _navigationService.CurrentView;

    public RelayCommand NavigateToTransactionsCommand { get; }
    public RelayCommand NavigateToSummaryCommand { get; }
    public RelayCommand NavigateToActivitiesCommand { get; }

    public NavigationViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;

        NavigateToTransactionsCommand = new RelayCommand(_ => _navigationService.NavigateTo<TransactionViewModel>());
        NavigateToSummaryCommand = new RelayCommand(_ => _navigationService.NavigateTo<SummaryViewModel>());
        NavigateToActivitiesCommand = new RelayCommand(_ => _navigationService.NavigateTo<ActivityViewModel>());

        // Imposta la vista iniziale
        _navigationService.NavigateTo<TransactionViewModel>();

        // Sottoscrivi i cambiamenti di CurrentView
        _navigationService.PropertyChanged += NavigationService_PropertyChanged;
    }

    private void NavigationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_navigationService.CurrentView))
        {
            OnPropertyChanged(nameof(CurrentView));
        }
    }
}