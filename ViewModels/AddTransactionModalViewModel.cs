using CashFlow.Core;
using CashFlow.Data;
using CashFlow.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class AddTransactionModalViewModel : ViewModel
{
    private readonly CashFlowContext _context;
    private readonly TransactionViewModel _parentViewModel;
    private readonly Action _closeWindow;

    private string _newDescription = string.Empty;
    private decimal _newAmount;
    private DateTime _newDate = DateTime.Today;
    private Account? _selectedAccount;
    private Activity? _selectedActivity;

    public ObservableCollection<Account> Accounts => _parentViewModel.Accounts;
    public ObservableCollection<Activity> Activities => _parentViewModel.Activities;

    public string NewDescription
    {
        get => _newDescription;
        set
        {
            _newDescription = value;
            OnPropertyChanged();
        }
    }

    public decimal NewAmount
    {
        get => _newAmount;
        set
        {
            _newAmount = value;
            OnPropertyChanged();
        }
    }

    public DateTime NewDate
    {
        get => _newDate;
        set
        {
            _newDate = value;
            OnPropertyChanged();
        }
    }

    public Account? SelectedAccount
    {
        get => _selectedAccount;
        set
        {
            _selectedAccount = value;
            OnPropertyChanged();
        }
    }

    public Activity? SelectedActivity
    {
        get => _selectedActivity;
        set
        {
            _selectedActivity = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddTransactionCommand { get; }

    public AddTransactionModalViewModel(CashFlowContext context, TransactionViewModel parentViewModel, Action closeWindow)
    {
        _context = context;
        _parentViewModel = parentViewModel;
        _closeWindow = closeWindow;

        // Inizializza i campi (già fatto dai valori di default)
        AddTransactionCommand = new RelayCommand(_ => AddTransaction(), _ => CanAddTransaction());
    }

    private void AddTransaction()
    {
        if (SelectedAccount == null || SelectedActivity == null) return;

        var transaction = new Transaction
        {
            Date = NewDate,
            Description = NewDescription,
            Amount = SelectedActivity.IsIncome ? Math.Abs(NewAmount) : -Math.Abs(NewAmount),
            AccountId = SelectedAccount.Id,
            ActivityId = SelectedActivity.Id,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        transaction.Account = SelectedAccount;
        transaction.Activity = SelectedActivity;

        // Aggiorna la DataGrid nel parent ViewModel
        _parentViewModel.Transactions.Add(transaction);

        // Mostra il messaggio di conferma
        var result = MessageBox.Show(
            "Transazione aggiunta con successo. Vuoi inserire un'altra transazione?",
            "Conferma",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        // Aggiorna i mesi e il filtro nel parent ViewModel
        _parentViewModel.ManageMonths(transaction.Date.ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture));

        if (result == MessageBoxResult.Yes)
        {
            // Resetta i campi per un nuovo inserimento
            ResetFields();
        }
        else
        {
            // Chiude la modale
            _closeWindow();
        }
    }

    private void ResetFields()
    {
        NewDescription = string.Empty;
        NewAmount = 0;
        NewDate = DateTime.Today;
        SelectedAccount = null;
        SelectedActivity = null;
    }

    private bool CanAddTransaction() =>
        !string.IsNullOrEmpty(NewDescription) && NewAmount != 0 &&
        SelectedAccount != null && SelectedActivity != null;
}