namespace EF_FluentAPIPropertyConfiguration.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,       // Payment initiated but not completed
        Paid = 2,          // Payment successful
        Failed = 3,        // Payment attempt failed
        Refunded = 4,      // Payment refunded after cancellation
        PartialRefund = 5  // Partial refund issued
    }
}
