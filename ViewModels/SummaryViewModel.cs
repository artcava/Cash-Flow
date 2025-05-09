using CashFlow.Core;
using CashFlow.Interfaces;
using System.Collections.ObjectModel;

namespace CashFlow.ViewModels;

public class SummaryViewModel : ViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    private ObservableCollection<YearlySummary> _summaries = new ObservableCollection<YearlySummary>();
    public ObservableCollection<YearlySummary> Summaries
    {
        get => _summaries;
        private set
        {
            _summaries = value;
            OnPropertyChanged();
        }
    }

    public SummaryViewModel(IUnitOfWork _uow)
    {
        _unitOfWork = _uow;
        LoadData();
    }

    private async Task LoadData()
    {
        Summaries.Clear();

        var transactions = await _unitOfWork.Transactions.GetAllAsync();

        var summaries = transactions.GroupBy(t => t.Date.Year)
            .Select(g => new YearlySummary
            {
                Year = g.Key,
                TotalIncome = g.Where(t => t.Amount > 0).Sum(t => t.Amount),
                TotalExpenses = g.Where(t => t.Amount < 0).Sum(t => t.Amount),
                Balance = g.Sum(t => t.Amount)
            })
            .OrderByDescending(s => s.Year)
            .ToList();

        foreach (var summary in summaries)
            Summaries.Add(summary);
    }
}

public class YearlySummary
{
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}