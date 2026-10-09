namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!; 

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
