using Microsoft.AspNetCore.Mvc;
using InforceAPI.Models;
using InforceAPI.Data;

namespace InforceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/Comments
    [HttpPost]
    public async Task<IActionResult> AddComment(int productId, string description)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound($"Product with ID {productId} not found.");
        }

        // Create the new comment and associate it with the product
        var comment = new Comment(productId, description);

        // Add the comment to the database
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Return the created comment
        return CreatedAtAction(nameof(AddComment), new { id = comment.Id }, comment);
    }


    // DELETE: api/Comments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
