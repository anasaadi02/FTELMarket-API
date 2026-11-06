using FTEMarket.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTEMarket.Api.Controllers;

public record CategoryResponse(int Id, string Name);

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(FtelMarketDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<CategoryResponse>> GetAll() =>
        await db.Categories
            .OrderBy(c => c.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync();
}
