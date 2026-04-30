namespace DbMigrator.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public ICollection<UserFavoriteCurrency> FavoriteCurrencies { get; set; } = new List<UserFavoriteCurrency>();
}
