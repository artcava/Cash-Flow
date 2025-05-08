using CashFlow.Core;

namespace CashFlow.Services;

//https://www.youtube.com/watch?v=wFzmBZpjuAo
public interface INavigationService
{
    ViewModel CurrentView { get; }
    void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModel;
}
public class NavigationService : ViewModel, INavigationService
{
    private readonly Func<Type, object, ViewModel> _viewModelFactory;
    private ViewModel _currentView;

    public ViewModel CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                OnPropertyChanged(); // Notify change for CurrentView
            }
        }
    }

    public NavigationService(Func<Type, object, ViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }


    public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModel
    {
        if (_currentView is IDisposable disposableView)
        {
            disposableView.Dispose();
        }

        var viewModel = _viewModelFactory.Invoke(typeof(TViewModel), parameter);

        CurrentView = viewModel;
    }

}
