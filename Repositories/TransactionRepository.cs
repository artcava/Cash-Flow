using CashFlow.Data;
using CashFlow.Interfaces;
using CashFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(CashFlowContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Transactions
            .Where(t => t.Date >= start && t.Date <= end)
            .Include(t => t.Account)
            .Include(t => t.Activity)
            .ToListAsync();
    }

    public async Task<decimal> GetBalanceAsync()
    {
        return await _context.Transactions.SumAsync(t => t.Amount);
    }
}