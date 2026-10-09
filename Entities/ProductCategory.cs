namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class ProductCategory
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public Product Product { get; set; } = null!;

        public Category Category { get; set; } = null!;

        public int DisplayOrder { get; set; } = 0; 

        public bool IsFeatured { get; set; } = false;

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
