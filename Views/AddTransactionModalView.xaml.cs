using CashFlow.ViewModels;
using System.Windows;

namespace CashFlow.Views;

public partial class AddTransactionModalView : Window
{
    public AddTransactionModalView(AddTransactionModalViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
