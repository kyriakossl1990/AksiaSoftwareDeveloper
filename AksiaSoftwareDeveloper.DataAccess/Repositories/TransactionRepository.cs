using AksiaSoftwareDeveloper.Common;
using AksiaSoftwareDeveloper.DataAccess.DBModels;
using AksiaSoftwareDeveloper.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AksiaSoftwareDeveloper.DataAccess.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context ?? throw new Exception("AppDbContext not found");
    }

    public async Task<PaginatedResponse<Transaction>> GetAllTransactions(int currentPage,
                                                                         int pageSize,
                                                                         CancellationToken ct)
    {
        var result = new PaginatedResponse<Transaction>();

        IQueryable<Transaction> query = _context.Transactions
            .AsNoTracking();

        result.TotalCount = await query.CountAsync();

        result.TotalPage = (int)Math.Ceiling(((double)result.TotalCount / pageSize));

        var skip = (currentPage - 1) * pageSize;

        result.Entities = await query
            .OrderByDescending(x => x.Inception)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return result;
    }

    public async Task<Transaction> GetTransactionById(Guid Id, CancellationToken ct)
    {
        return await _context.Transactions
           .AsNoTracking()
           .SingleOrDefaultAsync(t => Id == t.Id, ct)
            ?? throw new Exception($"Transaction with id:{Id} was not found");
    }

    public async Task<bool> UpdateTransaction(Transaction transaction, CancellationToken ct)
    {
        if (!ct.IsCancellationRequested)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync(ct);
        }
        return true;
    }

    public async Task DeleteTransactionById(Guid Id, CancellationToken ct)
    {
        if (!ct.IsCancellationRequested)
        {
            Transaction objToDelete = await _context.Transactions.SingleAsync(t => Id == t.Id);

            if (objToDelete is not null)
            {
                _context.Transactions.Remove(objToDelete);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
