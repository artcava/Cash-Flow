using CashFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Services;

public class ViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ViewModel CreateViewModel(Type viewModelType, object parameter = null)
    {
        if (parameter == null)
        {
            return (ViewModel)_serviceProvider.GetService(viewModelType);
        }
        else
        {
            return (ViewModel)ActivatorUtilities.CreateInstance(_serviceProvider, viewModelType, parameter);
        }
    }
}
