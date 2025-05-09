using CashFlow.Models;

namespace CashFlow.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<decimal> GetBalanceAsync();
}