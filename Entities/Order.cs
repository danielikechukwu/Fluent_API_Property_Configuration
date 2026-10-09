using EF_FluentAPIPropertyConfiguration.Enums;

namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class Order
    {
        public long Id { get; set; }

        public string OrderNumber { get; set; } = null!;

        public int CustomerId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public bool IsPriority { get; set; } = false;

        public string? ShippingAddress { get; set; }

        public string? BillingAddress { get; set; }

        public DateTime? CompletedDate { get; set; }

        public byte[] RowVersion { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Customer Customer { get; set; } = null!; 

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Payment? Payment { get; set; }
    }
}
