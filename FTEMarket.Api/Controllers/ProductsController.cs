using FTEMarket.Api.Data;
using FTEMarket.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTEMarket.Api.Controllers;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt);

public class ProductPayload
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [MaxLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
    public string Name { get; set; } = "";

    [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
    public string? Description { get; set; }

    [Range(0.01, 1_000_000, ErrorMessage = "Le prix doit être compris entre 0,01 et 1 000 000.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif.")]
    public int Stock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La catégorie est obligatoire.")]
    public int CategoryId { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController(FtelMarketDbContext db) : ControllerBase
{
    private static ProductResponse ToResponse(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.Category.Name, p.CreatedAt);

    private NotFoundObjectResult ProductNotFound(int id) => NotFound(new { message = $"Produit {id} introuvable." });

    [HttpGet]
    public async Task<IEnumerable<ProductResponse>> GetAll() =>
        await db.Products
            .OrderBy(p => p.Id)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock,
                p.CategoryId, p.Category.Name, p.CreatedAt))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? ProductNotFound(id) : ToResponse(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductPayload payload)
    {
        var category = await db.Categories.FindAsync(payload.CategoryId);
        if (category is null) return BadRequest(new { message = "Catégorie inexistante." });

        var product = new Product
        {
            Name = payload.Name,
            Description = payload.Description,
            Price = payload.Price,
            Stock = payload.Stock,
            Category = category,
            CreatedAt = DateTime.UtcNow,
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToResponse(product));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponse>> Update(int id, ProductPayload payload)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return ProductNotFound(id);

        var category = await db.Categories.FindAsync(payload.CategoryId);
        if (category is null) return BadRequest(new { message = "Catégorie inexistante." });

        product.Name = payload.Name;
        product.Description = payload.Description;
        product.Price = payload.Price;
        product.Stock = payload.Stock;
        product.Category = category;
        await db.SaveChangesAsync();
        return ToResponse(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await db.Products.Where(p => p.Id == id).ExecuteDeleteAsync() == 0
            ? ProductNotFound(id)
            : NoContent();
}
