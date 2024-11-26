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
    public async Task<IActionResult> GetPagedProducts(int pageNumber = 1, int pageSize = 10)
    {
        var products = await _context.Products
            .Include(p => p.Comments)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(products);
    }


    // GET: api/ProductById?Id=1
    [HttpGet]
    [Route("ProductById")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = _context.Products.Find(id);
        return product is not null ? Ok(product) : BadRequest("Product not found");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
    {
        if (createProductDto.Image == null || createProductDto.Image.Length == 0)
        {
            return BadRequest("Image is required.");
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
            Size = new Size(createProductDto.Width, createProductDto.Height),
            Weight = createProductDto.Weight
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPagedProducts), new { id = product.Id }, product);
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

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        if (id != updatedProduct.Id)
        {
            return BadRequest();
        }

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // Update fields
        existingProduct.ImageUrl = updatedProduct.ImageUrl;
        existingProduct.Name = updatedProduct.Name;
        existingProduct.Size = updatedProduct.Size;
        existingProduct.Weight = updatedProduct.Weight;

        _context.Entry(existingProduct).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(List<Tuple<int, int>> products_amount)
    {
        return Ok();
    }
}
