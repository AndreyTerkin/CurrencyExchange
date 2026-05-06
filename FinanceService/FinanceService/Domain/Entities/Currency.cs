namespace FinanceService.Domain.Entities;

public class Currency
{
    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal Rate { get; private set; }

    private Currency() { }

    public static Currency Create(int id, string code, string name, decimal rate) =>
        new() { Id = id, Code = code, Name = name, Rate = rate };
}
