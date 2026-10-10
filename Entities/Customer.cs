using EF_FluentAPIPropertyConfiguration.Enums;

namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerNumber { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FullName { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public CustomerType? CustomerType { get; set; }

        public bool IsActive { get; set; } = true; 

        public DateTime RegisteredAt { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] RowVersion { get; set; } = null!; 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();  // One customer can have many addresses

        public ICollection<Order> Orders { get; set; } = new List<Order>();         // One customer can have multiple orders

    }
}
