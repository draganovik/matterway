using Microsoft.EntityFrameworkCore;
using Payments.API.Entities;

namespace Payments.API.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payment { get; set; } = default!;
}
