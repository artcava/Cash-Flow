using CashFlow.Core;
using CashFlow.Data;
using CashFlow.Models;
using CashFlow.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class TransactionViewModel : ViewModel
{
    private readonly CashFlowContext _context;
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

    public TransactionViewModel(CashFlowContext context)
    {
        _context = context;

        LoadDataCommand = new RelayCommand(_ => LoadData());
        DeleteTransactionCommand = new RelayCommand(param => DeleteTransaction(param), param => CanDeleteTransaction(param));
        FilterByMonthCommand = new RelayCommand(param => FilterByMonth(param as string));
        OpenAddTransactionModalCommand = new RelayCommand(_ => OpenAddTransactionModal());


        LoadData();
    }

    private void LoadData()
    {
        Transactions.Clear();
        Accounts.Clear();
        Activities.Clear();
        Months.Clear();

        var transactions = _context.Transactions
            .Include(t => t.Activity)
            .Include(t => t.Account)
            .AsNoTracking()
            .ToList();

        var accounts = _context.Accounts.AsNoTracking().ToList();
        var activities = _context.Activities.AsNoTracking().ToList();

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
        //FilterByMonth(selectedMonth);
    }
    private void LoadMonths()
    {
        Months.Clear();
        var months = _context.Transactions
            .AsNoTracking()
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

    private void FilterByMonth(string? month)
    {
        if (string.IsNullOrEmpty(month)) return;

        Transactions.Clear();
        var date = DateTime.ParseExact(month, "MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
        var transactions = _context.Transactions
            .Include(t => t.Activity)
            .Include(t => t.Account)
            .AsNoTracking()
            .Where(t => t.Date.Year == date.Year && t.Date.Month == date.Month)
            .OrderByDescending(t => t.Date)
            .ToList();

        foreach (var transaction in transactions)
            Transactions.Add(transaction);
    }

    private void DeleteTransaction(object parameter)
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

        // Trova la transazione nel contesto per eliminarla
        var entity = _context.Transactions.Find(transaction.Id);
        if (entity != null)
        {
            _context.Transactions.Remove(entity);
            _context.SaveChanges();
        }

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
        var modalViewModel = new AddTransactionModalViewModel(_context, this, () => Application.Current.MainWindow.Dispatcher.Invoke(() =>
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
}