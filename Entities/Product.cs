namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string SKU { get; set; } = null!;

        public string? Description { get; set; } 

        public decimal Price { get; set; } 

        public decimal Discount { get; set; }

        public decimal FinalPrice { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public byte[] RowVersion { get; set; } = null!;

        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
