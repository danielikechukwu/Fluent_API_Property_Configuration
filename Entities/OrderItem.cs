namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class OrderItem
    {

        public int Id { get; set; }

        public long OrderId { get; set; } 

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TaxAmount { get; set; } 

        public decimal LineTotal { get; set; }

        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!; 
    }
}
