using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CashFlow.Data;

public class CashFlowContextFactory : IDesignTimeDbContextFactory<CashFlowContext>
{
    public CashFlowContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CashFlowContext>();
        optionsBuilder.UseSqlite("Data Source=cashflow.db");

        return new CashFlowContext(optionsBuilder.Options);
    }
}