namespace InforceAPI.Models;

public class Comment
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? Description { get; set; }
    public DateTime Date {  get; set; }

    public Comment(int productId, string? description)
    {
        ProductId = productId;
        Description = description;
        Date = DateTime.Now;
    }
}
