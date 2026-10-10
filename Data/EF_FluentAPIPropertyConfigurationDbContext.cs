using EF_FluentAPIPropertyConfiguration.Entities;
using EF_FluentAPIPropertyConfiguration.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace EF_FluentAPIPropertyConfiguration.Data
{
    public class EF_FluentAPIPropertyConfigurationDbContext : DbContext
    {
        public EF_FluentAPIPropertyConfigurationDbContext(DbContextOptions<EF_FluentAPIPropertyConfigurationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Configure primary key as identity column (INT IDENTITY(1,1) in SQL Server).
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key - auto-generated identity");

                // Business-facing customer code (separate from Id).
                // Stored as non-Unicode varchar(20) with a default static value.
                entity.Property(c => c.CustomerNumber)
                      .HasColumnName("Customer_Code")
                      .HasColumnType("varchar(20)")
                      .HasColumnOrder(1)
                      .IsRequired()
                      .HasMaxLength(20)
                      .IsUnicode(false)
                      .HasDefaultValue("NEW") // Assigned automatically for new customers
                      .HasComment("Unique customer code assigned by the business");

                entity.Property(c => c.FullName)
                      .HasMaxLength(200)
                      .IsUnicode(true) // Supports Unicode for international names
                      .HasComment("Full legal name of the customer (supports Unicode)");

                // Email is required, non-Unicode, and will be enforced as unique (index below).
                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(150)
                      .IsUnicode(false)
                      .HasComment("Unique email address used for login and notifications");

                entity.HasIndex(c => c.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Customer_Email");

                entity.Property(c => c.PhoneNumber)
                      .HasColumnType("varchar(10)")
                      .IsRequired(false)
                      .IsUnicode(false)
                      .HasComment("10-digit mobile number (no country code).");

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasColumnType("datetime2")
                      .HasComment("UTC timestamp when the customer record is first created.");

                entity.Property(c => c.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate()
                      .HasColumnType("datetime2")
                      .HasComment("UTC timestamp automatically refreshed whenever the record changes.");

                // Shadow Property

                entity.Property<DateTime>("LastModified")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate()
                      .HasColumnType("datetime2")
                      .HasComment("Shadow property used for internal audit tracking");

                entity.Property<bool>("IsActive")
                      .IsRequired()
                      .HasDefaultValue(true)
                      .HasComment("Indicates whether the customer account is active");

                entity.Property<bool>("IsDeleted")
                      .IsRequired()
                      .HasDefaultValue(false)
                      .HasComment("Soft delete flag for safe record removal");

                // Enum to string conversion for readability in DB (Regular/Premium/VIP).
                entity.Property(c => c.CustomerType)
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .IsUnicode(false)
                      .HasComment("Customer type: Regular, Premium, or VIP.");

                entity.Property(c => c.RowVersion)
                      .ValueGeneratedOnUpdate()
                      .IsConcurrencyToken()
                      .HasComment("Concurrency token to prevent conflicting updates");

                // Composite index for name + phone with included Email for covering queries.
                entity.HasIndex(c => new { c.FullName, c.PhoneNumber })
                      .HasDatabaseName("IX_Customers_NamePhone")
                      .IncludeProperties(c => c.Email);

            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Identity primary key for Address table.
                entity.Property(a => a.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key for Address.");

                // Human-readable address parts with Unicode support.
                entity.Property(a => a.Street)
                      .HasMaxLength(200)
                      .IsUnicode(true)
                      .HasComment("Street and house/flat details.");

                entity.Property(a => a.City)
                      .HasMaxLength(100)
                      .IsUnicode(true)
                      .HasComment("City or town name.");

                entity.Property(a => a.State)
                     .HasMaxLength(100)
                     .IsUnicode(true)
                     .HasComment("State or province name.");

                // Zip/Pin stored as fixed length char(6) to match Indian PIN codes.
                entity.Property(a => a.ZipCode)
                      .HasColumnType("char(6)")
                      .IsUnicode(false)
                      .HasComment("Postal/PIN code.");

                entity.Property(a => a.Country)
                      .HasMaxLength(100)
                      .IsUnicode(true)
                      .HasComment("Country name.");

                // Flag to indicate primary/default address for a customer.
                entity.Property(a => a.IsDefault)
                      .HasDefaultValue(false)
                      .HasComment("True if this address is used as default shipping/billing");

                // Shadow property for audit – created date handled entirely by DB.
                entity.Property<DateTime>("CreatedAt")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasComment("UTC timestamp when address was created");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Identity primary key for Product.
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key - auto identity column for Product.");
                // SKU (Stock Keeping Unit) is a business key used in inventory systems.
                entity.Property(p => p.SKU)
                      .HasColumnType("varchar(30)")
                      .IsUnicode(false)
                      .IsRequired()
                      .HasComment("Stock-keeping unit code. Typically unique and used in inventory/barcodes.");

                entity.Property(p => p.Name)
                      .HasMaxLength(150)
                      .IsUnicode(true)
                      .IsRequired()
                      .HasComment("Product display name.");

                entity.Property(p => p.Description)
                      .HasMaxLength(500)
                      .IsUnicode(true)
                      .HasComment("Short description shown on product page.");

                // Price and Discount with precision ensure proper monetary storage.
                entity.Property(p => p.Price)
                      .HasPrecision(10, 2)
                      .HasDefaultValue(0.00m)
                      .HasComment("Base price of the product before any discount.");

                entity.Property(p => p.Discount)
                      .HasPrecision(10, 2)
                      .HasDefaultValue(0.00m)
                      .HasComment("Flat discount amount applied on the base price.");


                // FinalPrice is computed by SQL Server, persisted as a physical column.
                entity.Property(p => p.FinalPrice)
                      .HasPrecision(10, 2)
                      .HasComputedColumnSql("[Price] - [Discount]", stored: true)
                      .HasComment("Persisted computed column: FinalPrice = Price - Discount.");

                entity.Property(p => p.StockQuantity)
                      .HasDefaultValue(0)
                      .HasComment("Available stock quantity for this product.");

                // Audit timestamps maintained by the database.
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasComment("Timestamp (UTC) when product was created.");

                entity.Property(p => p.UpdatedAt)
                     .HasDefaultValueSql("GETUTCDATE()")
                     .ValueGeneratedOnAddOrUpdate()
                     .HasComment("Timestamp (UTC) automatically updated on product changes.");

                // Soft-state flags for enabling/disabling products and soft deletes.
                entity.Property(p => p.IsActive)
                      .HasDefaultValue(true)
                      .HasComment("False means product is hidden from customer view");

                entity.Property(p => p.IsDeleted)
                    .HasDefaultValue(false)
                    .HasComment("Soft delete for product archiving");

                // RowVersion column for optimistic concurrency checks on Product updates.
                entity.Property(p => p.RowVersion)
                      .IsConcurrencyToken()
                      .HasComment("RowVersion for optimistic concurrency control.");

                // Index to quickly fetch active products by SKU,
                // including Price and StockQuantity as covered columns.
                entity.HasIndex(p => new { p.SKU, p.IsActive })
                      .HasDatabaseName("IX_Products_ActiveSKU")
                      .IncludeProperties(e => new { e.Price, e.StockQuantity });

            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key for Category.");

                entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired()
                .HasComment("Category name (e.g., Electronics, Clothing).");

                entity.Property(c => c.Description)
                      .HasMaxLength(300)
                      .IsUnicode(true)
                      .HasComment("Optional description for this category.");

                entity.Property(c => c.IsActive)
                      .HasDefaultValue(true)
                      .HasComment("Inactive categories are hidden from UI");

                // Shadow CreatedAt for audit, fully handled by DB.
                entity.Property<DateTime>("CreatedAt")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasComment("UTC timestamp when category was created.");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                // When product is linked to category, this timestamp is captured by DB.
                entity.Property(c => c.LinkedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .HasComment("Timestamp when the product was assigned to this category");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key for Order.");

                // OrderNumber is a business identifier (e.g., 'ORD20250001') generated by app/service.
                // ValueGeneratedNever ensures EF does not try to auto-generate this.
                entity.Property(o => o.OrderNumber)
                      .HasColumnType("varchar(12)")
                      .IsUnicode(false)
                      .IsRequired()
                      .ValueGeneratedNever()
                      .HasComment("Business-generated order number (not auto-increment)");

                // SubTotal, TaxAmount, Discount, TotalAmount are monetary fields.
                entity.Property(o => o.SubTotal)
                      .HasPrecision(18, 2)
                      .HasDefaultValue(0.00m)
                      .HasComment("Order subtotal before tax and discount.");

                entity.Property(o => o.TaxAmount)
                      .HasPrecision(10, 2)
                      .HasDefaultValueSql("0.00")
                      .HasComment("Total tax applied on this order.");

                entity.Property(o => o.Discount)
                      .HasPrecision(10, 2)
                      .HasDefaultValueSql("0.00")
                      .HasComment("Total discount applied at order level.");

                // TotalAmount stored as a computed column by SQL Server.
                entity.Property(o => o.TotalAmount)
                      .HasPrecision(18, 2)
                      .HasComputedColumnSql("[SubTotal] + [TaxAmount] - [Discount]", stored: true)
                      .HasComment("Persisted computed column: TotalAmount = SubTotal + TaxAmount - Discount");

                // CreatedAt – first time order is created.
                entity.Property(o => o.CreatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasComment("Order creation timestamp (UTC).");

                // UpdatedAt also serves as a concurrency token to detect stale updates.
                entity.Property(o => o.UpdatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate()
                      .IsConcurrencyToken()
                      .HasComment("Order last updated timestamp (UTC), also used for concurrency control.");

                // ShippedDate is optional and stored as local time using GETDATE().
                entity.Property(o => o.ShippedDate)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()")
                      .IsRequired(false)
                      .HasComment("Date and time when order is shipped (server local time).");

                // CompletedDate only stores the date part (no time).
                entity.Property(o => o.CompletedDate)
                      .HasColumnType("date")
                      .IsRequired(false)
                      .HasComment("Date when order is fully completed/closed.");

                // Enum stored as integer for OrderStatus.
                entity.Property(o => o.OrderStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(OrderStatus.Pending)
                      .HasComment("Order status: Pending=0, Processing=1, Shipped=2, Completed=3, Cancelled=4");

                // RowVersion concurrency token separate from UpdatedAt.
                entity.Property(o => o.RowVersion)
                      .IsConcurrencyToken()
                      .HasComment("Concurrency field to detect conflicting writes");

                // Shadow property to track last time this order was accessed/read.
                entity.Property<DateTime>("LastAccessed")
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate()
                      .HasComment("Shadow property used for audit/reporting");

                // Unique index on OrderNumber to prevent duplicates and speed lookups.
                entity.HasIndex(o => o.OrderNumber)
                      .IsUnique()
                      .HasDatabaseName("IX_Orders_OrderNumber");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.Id)
                    .ValueGeneratedOnAdd()
                    .HasComment("Primary key for OrderItem.");

                // Default quantity is 1 to simplify single item additions.
                entity.Property(oi => oi.Quantity)
                      .HasDefaultValue(1)
                      .HasComment("Number of units of the product in this line item.");

                entity.Property(oi => oi.UnitPrice)
                      .HasPrecision(18, 2)
                      .HasDefaultValue(0.00m)
                      .HasComment("Price per unit for this product at the time of order.");

                entity.Property(oi => oi.TaxAmount)
                      .HasPrecision(18, 2)
                      .HasDefaultValue(0.00m)
                      .HasComment("Tax amount applied for this line item.");

                // LineTotal calculated by SQL Server and persisted for faster reads.
                entity.Property(oi => oi.LineTotal)
                      .HasPrecision(18, 2)
                      .HasComputedColumnSql("([Quantity] * [UnitPrice]) + [TaxAmount]", stored: true)
                      .HasComment("Line total = Quantity * UnitPrice + TaxAmount (computed column).");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd()
                      .HasComment("Primary key for Payment.");


                // PaymentStatus enum stored as string for readability and easier debugging.
                entity.Property(p => p.PaymentStatus)
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .IsUnicode(false)
                      .HasDefaultValue(PaymentStatus.Pending)
                      .HasComment("Payment state as text (Pending, Completed, Failed, Refunded, etc.).");

                entity.Property(p => p.Amount)
                      .HasPrecision(18, 2)
                      .HasComment("Total payment amount processed for the order.");

                // PaidAt is nullable since the payment might not be completed yet.
                entity.Property(p => p.PaidAt)
                      .HasColumnType("datetime2")
                      .IsRequired(false)
                      .HasComment("Date and time when payment was successfully completed (if any).");

                // RowVersion for concurrency – helps avoid conflicting updates.
                entity.Property(p => p.RowVersion)
                      .IsConcurrencyToken()
                      .ValueGeneratedOnAddOrUpdate()
                      .HasComment("RowVersion used for optimistic concurrency on payments.");

                // UpdatedAt acts as a simple timestamp-based concurrency token.
                entity.Property(p => p.UpdatedAt)
                      .IsConcurrencyToken()
                      .HasDefaultValueSql("GETUTCDATE()")
                      .HasComment("Used to detect conflicting updates to payment state");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(a => a.Id)
                    .ValueGeneratedOnAdd()
                    .HasComment("Primary key for AuditLog entries.");

                entity.Property(a => a.ActionType)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasComment("Type of action performed (INSERT, UPDATE, DELETE)");

                entity.Property(a => a.TableName)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasComment("Name of the table/entity affected by the action");

                // Defaults to 'System' when no explicit user is supplied.
                entity.Property(a => a.PerformedBy)
                      .HasColumnType("varchar(50)")
                      .IsUnicode(false)
                      .HasDefaultValue("System")
                      .HasComment("Username or system identifier that performed the action.");

                // ActionTime is database-generated to ensure reliable auditing.
                entity.Property(a => a.ActionTime)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd()
                      .HasComment("Timestamp when the action was performed");
            });
        
        }
    
    }
}
