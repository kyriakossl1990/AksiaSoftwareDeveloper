using Microsoft.EntityFrameworkCore;
using AksiaSoftwareDeveloper.DataAccess.DBModels;

namespace AksiaSoftwareDeveloper.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    }
}
