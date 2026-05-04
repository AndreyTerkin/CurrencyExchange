using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
