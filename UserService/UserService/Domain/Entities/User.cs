namespace UserService.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    private User() { }

    public static User Create(string name, string passwordHash) =>
        new() { Name = name, PasswordHash = passwordHash };
}
