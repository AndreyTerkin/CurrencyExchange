namespace CurrencyWorker.Persistence;

public class CurrencyEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Rate { get; set; }

    private CurrencyEntity() { }

    public static CurrencyEntity Create(string code, string name, decimal rate) =>
        new() { Code = code, Name = name, Rate = rate };
}
