using EF_FluentAPIPropertyConfiguration.Enums;

namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class Payment
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Provider { get; set; } = null!;

        public string PaymentReference { get; set; } = null!;

        public PaymentStatus? PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public byte[] RowVersion { get; set; } = null!;

        public string? MetadataJson { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Order Order { get; set; } = null!;

    }
}
