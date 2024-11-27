using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InforceAPI.Models;
using InforceAPI.Data;

namespace InforceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: api/Products?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetPagedProducts(int pageNumber = 1, int pageSize = 10, string sortBy = "name")
    {
        IQueryable<Product> query = _context.Products.Include(p => p.Comments);
        switch (sortBy.ToLower())
        {
            case "name":
                query = query.OrderBy(p => p.Name).ThenBy(p => p.Count);
                break;
            case "count":
                query = query.OrderBy(p => p.Count).ThenBy(p => p.Name);
                break;
            default:
                query = query.OrderBy(p => p.Name).ThenBy(p => p.Count);
                break;
        }

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(products);
    }


    // GET: api/Products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _context.Products
                                    .Include(p => p.Comments) // Eagerly load comments
                                    .FirstOrDefaultAsync(p => p.Id == id);

        return product is not null ? Ok(product) : NotFound("Product not found");
    }


    // POST: api/Products
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
    {
        if (createProductDto.Image == null || createProductDto.Image.Length == 0)
        {
            return BadRequest("Image is required when creating a product.");
        }

        // Generate a new GUID-based file name
        var fileExtension = Path.GetExtension(createProductDto.Image.FileName);
        var newFileName = $"{Guid.NewGuid()}{fileExtension}";

        // Define the path to save the image in wwwroot
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(uploadsFolder, newFileName);

        // Save the image to the wwwroot/uploads folder
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await createProductDto.Image.CopyToAsync(stream);
        }

        // Create the product entity
        var product = new Product
        {
            Name = createProductDto.Name,
            ImageUrl = $"/uploads/{newFileName}",
            Count = createProductDto.Count,
            Size = new Size(createProductDto.Width, createProductDto.Height),
            Weight = createProductDto.Weight
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    // DELETE: api/Products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Optionally delete the image file
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var filePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateProductDto updatedProductDto)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return NotFound("Product not found.");
        }

        // Check if a new image is provided
        if (updatedProductDto.Image != null && updatedProductDto.Image.Length > 0)
        {
            // Generate a new GUID-based file name for the new image
            var fileExtension = Path.GetExtension(updatedProductDto.Image.FileName);
            var newFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Define the path to save the new image in wwwroot
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, newFileName);

            // Save the new image to the file system
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await updatedProductDto.Image.CopyToAsync(stream);
            }

            // Delete the old image file if it exists
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var oldFilePath = Path.Combine(_environment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Update the product's ImageUrl with the new image path
            existingProduct.ImageUrl = $"/uploads/{newFileName}";
        }

        // Update other product fields
        existingProduct.Name = updatedProductDto.Name;
        existingProduct.Size = new Size(updatedProductDto.Width, updatedProductDto.Height);
        existingProduct.Weight = updatedProductDto.Weight;
        existingProduct.Count = updatedProductDto.Count;

        _context.Entry(existingProduct).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(_context.Products.Find(id));
    }

}
