using AksiaSoftwareDeveloper.Api.Middlewares;
using AksiaSoftwareDeveloper.Common.DTO;
using AksiaSoftwareDeveloper.Core.Interfaces;
using AksiaSoftwareDeveloper.Core.Services;
using AksiaSoftwareDeveloper.DataAccess;
using AksiaSoftwareDeveloper.DataAccess.DBModels;
using AksiaSoftwareDeveloper.DataAccess.Interfaces;
using AksiaSoftwareDeveloper.DataAccess.Repositories;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await SeedData(app);

app.UseHttpsRedirection();

app.MapGet("/GetAllTransactions/{currentPage}/{pageSize}", async (int currentPage, int pageSize, [FromServices] ITransactionService service, CancellationToken ct) =>
{
    return Results.Ok(await service.GetAllTransactions(currentPage, pageSize, ct));
})
.WithName("GetAllTransactions")
.WithOpenApi();

app.MapGet("/GetTransaction/{id}", async (Guid Id, [FromServices] ITransactionService service, CancellationToken ct) =>
{
    return Results.Ok(await service.GetTransactionById(Id, ct));
})
.WithName("GetTransaction")
.WithOpenApi();

app.MapDelete("/DeleteTransaction/{id}", async (Guid Id, [FromServices] ITransactionService service, CancellationToken ct) =>
{
    await service.DeleteTransactionById(Id, ct);
    return Results.Ok();
})
.WithName("DeleteTransaction")
.WithOpenApi();

app.MapPut("/UpdateTransactions", async (TransactionDTO transaction, [FromServices] ITransactionService service, CancellationToken ct) =>
{
    await service.UpdateTransaction(transaction, ct);
    return Results.Ok();
})
.WithName("UpdateTransactions")
.WithOpenApi();

app.MapGet("/TestError", async ([FromServices] ITransactionService service, CancellationToken ct) =>
{
    return Results.Ok(await service.GetTransactionById(Guid.NewGuid(), ct));
})
.WithName("TestError")
.WithOpenApi();

app.Run();

static async Task SeedData(WebApplication app)
{
    using (var Scope = app.Services.CreateScope())
    {
        var context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        var needToSeedData = !await context.Transactions
            .AsNoTracking()
            .AnyAsync();

        if (needToSeedData)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string file = string.Format("{0}\\MOCK_DATA .csv", path);

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                IEnumerable<TransactionMap> records = csv.GetRecords<TransactionMap>();
                List<Transaction> transactionsToSeed = new List<Transaction>();

                foreach (var tran in records)
                {
                    transactionsToSeed.Add(new Transaction
                    {
                        Id = Guid.TryParse(tran.Id, out var transactionId) ? transactionId : Guid.NewGuid(),
                        Amount = tran.Amount ?? $"${new Random().Next(100)}",
                        ApplicationName = tran.ApplicationName,
                        Email = tran.Email,
                        Allocation = decimal.TryParse(tran.Allocation, out var res) ? res : 0,
                        Filename = tran.Filename ?? string.Empty,
                        Inception = DateTime.TryParse(tran.Inception, out var dateres) ? dateres : DateTime.UtcNow.AddDays(-10),
                        Url = tran.Url ?? string.Empty
                    });
                }
                await context.AddRangeAsync(transactionsToSeed);
                await context.SaveChangesAsync();
            }
        }
    }
}

public record TransactionMap
{
    public string Id { get; set; }
    public string ApplicationName { get; set; }
    public string Email { get; set; }
    public string Filename { get; set; }
    public string Url { get; set; }
    public string Inception { get; set; }
    public string Amount { get; set; }
    public string Allocation { get; set; }
}