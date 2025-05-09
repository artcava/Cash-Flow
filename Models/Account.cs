using CashFlow.Interfaces;

namespace CashFlow.Models;

public class Account : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}