using System.IdentityModel.Tokens.Jwt;
using FinanceService.Application.Queries.GetUserFavoriteCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Controllers;

[ApiController]
[Route("api/currencies")]
[Authorize]
public class CurrenciesController(IMediator mediator) : ControllerBase
{
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites(CancellationToken ct)
    {
        string? subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out int userId))
            return Unauthorized();

        IReadOnlyList<CurrencyDto> result = await mediator.Send(new GetUserFavoriteCurrenciesQuery(userId), ct);
        return Ok(result);
    }
}
