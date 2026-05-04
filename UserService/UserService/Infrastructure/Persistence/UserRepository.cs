using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Persistence;

public class UserRepository(UserDbContext db) : IUserRepository
{
    public Task<User?> GetByNameAsync(string name, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Name == name, ct);

    public Task<bool> ExistsAsync(string name, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Name == name, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
