namespace InforceAPI.Controllers;

public partial class ProductsController
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Count { get; set; }
        public IFormFile? Image { get; set; } // For image upload
    }
}
