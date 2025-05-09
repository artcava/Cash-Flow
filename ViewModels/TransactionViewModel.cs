using CashFlow.Core;
using CashFlow.Interfaces;
using CashFlow.Models;
using CashFlow.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class TransactionViewModel : ViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    
    private string? _selectedMonth;
    private Transaction? _selectedTransaction;

    private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();
    private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();
    private ObservableCollection<Activity> _activities = new ObservableCollection<Activity>();
    private ObservableCollection<string> _months = new ObservableCollection<string>();

    public ObservableCollection<Transaction> Transactions
    {
        get => _transactions;
        private set
        {
            _transactions = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Account> Accounts
    {
        get => _accounts;
        private set
        {
            _accounts = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Activity> Activities
    {
        get => _activities;
        private set
        {
            _activities = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Months
    {
        get => _months;
        private set
        {
            _months = value;
            OnPropertyChanged();
        }
    }

    public string? SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            _selectedMonth = value;
            OnPropertyChanged();
            FilterByMonth(value);
        }
    }

    public Transaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set
        {
            _selectedTransaction = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand DeleteTransactionCommand { get; }
    public ICommand FilterByMonthCommand { get; }
    public ICommand OpenAddTransactionModalCommand { get; }
    public ICommand OpenEditTransactionModalCommand { get; }

    public TransactionViewModel(IUnitOfWork _uow)
    {
        _unitOfWork = _uow;

        LoadDataCommand = new RelayCommand(_ => LoadData());
        DeleteTransactionCommand = new RelayCommand(param => DeleteTransaction(param), param => CanDeleteTransaction(param));
        FilterByMonthCommand = new RelayCommand(param => FilterByMonth(param as string));
        OpenAddTransactionModalCommand = new RelayCommand(_ => OpenAddTransactionModal());
        OpenEditTransactionModalCommand = new RelayCommand(_ => OpenEditTransactionModal(), _ => SelectedTransaction != null);


        LoadData();
    }

    private async void LoadData()
    {
        Transactions.Clear();
        Accounts.Clear();
        Activities.Clear();
        Months.Clear();

        var transactions = await _unitOfWork.Transactions.GetAllWithIncludeAsync(t => t.Activity, t => t.Account);

        var accounts = await _unitOfWork.Accounts.GetAllAsync();
        var activities = await _unitOfWork.Activities.GetAllAsync();

        foreach (var transaction in transactions)
            Transactions.Add(transaction);

        foreach (var account in accounts)
            Accounts.Add(account);

        foreach (var activity in activities)
            Activities.Add(activity);

        LoadMonths();

        SelectedMonth = Months.FirstOrDefault();
    }

    public void ManageMonths(string month)
    {
        LoadMonths();
        SelectedMonth = Months.FirstOrDefault(m => m == month) ?? Months.FirstOrDefault();
    }
    private void LoadMonths()
    {
        Months.Clear();

        var months = Transactions
            .Select(t => new { t.Date.Year, t.Date.Month })
            .Distinct()
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList()
            .Select(m => new DateTime(m.Year, m.Month, 1).ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture))
            .ToList();

        foreach (var month in months)
            Months.Add(month);
    }

    private async Task FilterByMonth(string? month)
    {
        if (string.IsNullOrEmpty(month)) return;

        Transactions.Clear();
        var date = DateTime.ParseExact(month, "MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

        var transactions = await _unitOfWork.Transactions.FindAsync(t => t.Date.Year == date.Year && t.Date.Month == date.Month, t => t.Activity, testc => testc.Account);

        foreach (var transaction in transactions.OrderByDescending(t => t.Date))
            Transactions.Add(transaction);
    }

    private async Task DeleteTransaction(object parameter)
    {
        var transaction = parameter as Transaction;
        if (transaction == null) return;

        // Mostra il messaggio di conferma
        var result = MessageBox.Show(
            "Sei sicuro di voler eliminare questa transazione?",
            "Conferma Eliminazione",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        // Elimina la transazione nel contesto
        await _unitOfWork.Transactions.DeleteAsync(transaction.Id);

        // Rimuove la transazione dalla collezione
        Transactions.Remove(transaction);
        SelectedTransaction = null;

        // Aggiorna i mesi
        LoadMonths();
        if (!Months.Contains(SelectedMonth))
            SelectedMonth = Months.FirstOrDefault();
        FilterByMonth(SelectedMonth);
    }
    private bool CanDeleteTransaction(object parameter) => parameter is Transaction;

    private void OpenAddTransactionModal()
    {
        var modalViewModel = new AddTransactionModalViewModel(_unitOfWork, this, () => Application.Current.MainWindow.Dispatcher.Invoke(() =>
        {
            var modal = Application.Current.Windows.OfType<AddTransactionModalView>().FirstOrDefault();
            modal?.Close();
        }));
        var modalView = new AddTransactionModalView(modalViewModel)
        {
            Owner = Application.Current.MainWindow
        };
        modalView.ShowDialog();
    }
    private void OpenEditTransactionModal()
    {
        if (SelectedTransaction == null) return;

        var modalViewModel = new AddTransactionModalViewModel(
            _unitOfWork,
            this,
            () => Application.Current.MainWindow.Dispatcher.Invoke(() =>
            {
                var modal = Application.Current.Windows.OfType<AddTransactionModalView>().FirstOrDefault();
                modal?.Close();
            }),
            SelectedTransaction); // Passa la transazione selezionata

        var modalView = new AddTransactionModalView(modalViewModel)
        {
            Owner = Application.Current.MainWindow,
            Title = "Modifica Transazione" // Cambia il titolo
        };
        modalView.ShowDialog();
    }
}