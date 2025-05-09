using CashFlow.Core;
using CashFlow.Interfaces;
using CashFlow.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class AddTransactionModalViewModel : ViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransactionViewModel _parentViewModel;
    private readonly Action _closeWindow;

    private string _newDescription = string.Empty;
    private decimal _newAmount;
    private DateTime _newDate = DateTime.Today;
    private Account? _selectedAccount;
    private Activity? _selectedActivity;

    private Transaction? _transactionToEdit;
    private bool _isEditMode;

    public string WindowTitle => _isEditMode ? "Modifica Transazione" : "Aggiungi Transazione";
    public string ActionButtonText => _isEditMode ? "Salva" : "Aggiungi";
    public ICommand ActionCommand { get; }

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

    public AddTransactionModalViewModel(IUnitOfWork _uow, TransactionViewModel parentViewModel, Action closeWindow, Transaction? transactionToEdit = null)
    {
        _unitOfWork = _uow;
        _parentViewModel = parentViewModel;
        _closeWindow = closeWindow;
        _transactionToEdit = transactionToEdit;
        _isEditMode = transactionToEdit != null;
        if (_isEditMode)
        {
            // Inizializza i campi con i valori della transazione da modificare
            NewDescription = transactionToEdit.Description;
            NewAmount = Math.Abs(transactionToEdit.Amount);
            NewDate = transactionToEdit.Date;
            SelectedAccount = Accounts.FirstOrDefault(a => a.Id == _transactionToEdit.AccountId);
            SelectedActivity = Activities.FirstOrDefault(a => a.Id == _transactionToEdit.ActivityId);
        }

        ActionCommand = new RelayCommand(_ => ExecuteAction(), _ => CanExecuteAction());
    }

    private async void ExecuteAction()
    {
        if (_isEditMode)
            await UpdateTransaction();
        else
            await AddTransaction();
    }

    private async Task AddTransaction()
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

        await _unitOfWork.Transactions.AddAsync(transaction);

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
    private async Task UpdateTransaction()
    {
        try
        {
            if (_transactionToEdit == null || SelectedAccount == null || SelectedActivity == null) return;

            var transaction = await _unitOfWork.Transactions.GetByIdAsync(_transactionToEdit.Id);
            if (transaction == null) return;

            transaction.Date = NewDate;
            transaction.Description = NewDescription;
            transaction.Amount = SelectedActivity.IsIncome ? Math.Abs(NewAmount) : -Math.Abs(NewAmount);
            transaction.AccountId = SelectedAccount.Id;
            transaction.ActivityId = SelectedActivity.Id;

            await _unitOfWork.Transactions.UpdateAsync(transaction);

            // Aggiorna la UI
            _parentViewModel.ManageMonths(transaction.Date.ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture));

            _closeWindow();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore durante l'aggiornamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
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

    private bool CanExecuteAction() =>
        !string.IsNullOrEmpty(NewDescription) &&
        NewAmount != 0 &&
        SelectedAccount != null &&
        SelectedActivity != null;
}