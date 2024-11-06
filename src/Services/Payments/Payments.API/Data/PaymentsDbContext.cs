using Microsoft.EntityFrameworkCore;
using Payments.API.Entities;
using Shared.Enums;

namespace Payments.API.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payment { get; set; } = default!;

    // add data to database
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                ReferenceNumber = "5655-6666-7877",
                CardHolder = "Mara Jakov",
                CardNumber = "1234-5678-1234-5678",
                SecurityCode = "1234",
                ExpirationDate = "12/26",
                PaymentDate = DateTime.Now.Subtract(TimeSpan.FromHours(3)),
                PaymentState = PaymentState.Processed,
                PaymentAmount = 39998
            },
            new Payment
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                ReferenceNumber = "6666-8888-6588",
                CardHolder = "Stefan Stefanov",
                CardNumber = "8856-5678-1234-3366",
                SecurityCode = "6658",
                ExpirationDate = "06/24",
                PaymentDate = DateTime.Now,
                PaymentState = PaymentState.Processed,
                PaymentAmount = 4999
            }
        );
    }
}