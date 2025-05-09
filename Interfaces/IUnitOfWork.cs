using CashFlow.Models;

namespace CashFlow.Interfaces;

public interface IUnitOfWork: IDisposable
{
    ITransactionRepository Transactions { get; }
    IRepository<Account> Accounts { get; }
    IRepository<Activity> Activities { get; }
    Task<int> SaveChangesAsync();
}