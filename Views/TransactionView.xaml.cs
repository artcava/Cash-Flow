using CashFlow.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace CashFlow.Views;

public partial class TransactionView : UserControl
{
    public TransactionView()
    {
        InitializeComponent();
    }
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is TransactionViewModel vm && vm.SelectedTransaction != null)
        {
            vm.OpenEditTransactionModalCommand.Execute(null);
        }
    }
}