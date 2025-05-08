using Microsoft.EntityFrameworkCore;
using CashFlow.Models;

namespace CashFlow.Data;

public class CashFlowContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Activity> Activities { get; set; }

    public CashFlowContext(DbContextOptions<CashFlowContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Activity)
            .WithMany()
            .HasForeignKey(t => t.ActivityId);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Date);
        
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.ActivityId);
        // Dati iniziali
        modelBuilder.Entity<Account>().HasData(
            new Account { Id = 1, Name = "Conto Corrente" },
            new Account { Id = 2, Name = "Cassa" }
        );

        modelBuilder.Entity<Activity>().HasData(
            new Activity { Id = 1, Name = "Vendite", IsIncome = true },
            new Activity { Id = 2, Name = "Spese operative", IsIncome = false },
            new Activity { Id = 3, Name = "Manutenzione", IsIncome = false }
        );
    }
}