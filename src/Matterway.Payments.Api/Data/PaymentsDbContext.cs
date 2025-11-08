using Microsoft.EntityFrameworkCore;
using Matterway.Common.Enums;
using Matterway.Payments.Api.Features.Payments.Domain;

namespace Matterway.Payments.Api.Data;

public class PaymentsDb : DbContext
{
    public PaymentsDb(DbContextOptions<PaymentsDb> options)
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
                PaymentDate = new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc),
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
                PaymentDate = new DateTime(2024, 6, 2, 11, 30, 0, DateTimeKind.Utc),
                PaymentState = PaymentState.Processed,
                PaymentAmount = 4999
            }
        );
    }
}