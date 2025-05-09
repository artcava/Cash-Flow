using CashFlow.Data;
using CashFlow.Interfaces;
using CashFlow.Models;
using CashFlow.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Uow;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContextFactory<CashFlowContext> _contextFactory;
    private CashFlowContext _context;
    private bool _disposed = false;

    public UnitOfWork(IDbContextFactory<CashFlowContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _context = _contextFactory.CreateDbContext();

        Transactions = new TransactionRepository(_context);
        Accounts = new Repository<Account>(_context);
        Activities = new Repository<Activity>(_context);
    }
    public ITransactionRepository Transactions { get; }

    public IRepository<Account> Accounts { get; }

    public IRepository<Activity> Activities { get; }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context?.Dispose();
            _disposed = true;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch
        {
            // In caso di errore, ricrea il contesto
            if (_context != null)
            {
                await _context.DisposeAsync();
            }
            _context = _contextFactory.CreateDbContext();
            throw;
        }
    }
}
