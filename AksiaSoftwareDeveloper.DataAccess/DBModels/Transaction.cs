using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AksiaSoftwareDeveloper.DataAccess.DBModels;

public class Transaction :
    DBEntity,
    IEntityTypeConfiguration<Transaction>
{
    public required string ApplicationName { get; set; }
    public required string Email { get; set; }
    public string? Filename { get; set; }
    public string? Url { get; set; }
    public DateTime? Inception { get; set; }
    public required string Amount { get; set; }
    public decimal Allocation { get; set; }

    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(p => p.ApplicationName).HasMaxLength(200);
        builder.Property(p => p.Email).HasMaxLength(200);
        builder.Property(p => p.Filename).HasMaxLength(300);
        builder.Property(p => p.Allocation).HasPrecision(0, 100);
    }
}

