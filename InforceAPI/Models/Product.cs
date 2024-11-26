namespace InforceAPI.Models;

public class Product
{
    public int Id { get; set; } 
    public string? ImageUrl { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public Size Size { get; set; }  
    public double Weight {  get; set; }
    public List<Comment>? Comments { get; set; }

    // Parameterless constructor for EF Core
    public Product()
    {
    }

    public Product(string? imageUrl, string name, int count, Size size, double weight)
    {
        ImageUrl = imageUrl;
        Name = name;
        Count = count;
        Size = size;
        Weight = weight;
    }

    public Product(string? imageUrl, string name, int count, double width, double height, double weight)
    {
        ImageUrl = imageUrl;
        Name = name;
        Count = count;
        Size = new Size(width, height);
        Weight = weight;
    }
}


