using CashFlow.Interfaces;

namespace CashFlow.Models;

public class Activity : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIncome { get; set; } // True = attivo (incasso), False = passivo (spesa)
}