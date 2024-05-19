using AksiaSoftwareDeveloper.Common;
using AksiaSoftwareDeveloper.DataAccess.DBModels;

namespace AksiaSoftwareDeveloper.DataAccess.Interfaces;

public interface ITransactionRepository
{
    Task<PaginatedResponse<Transaction>> GetAllTransactions(int currentPage, int pageSize, CancellationToken ct);

    Task<Transaction> GetTransactionById(Guid Id, CancellationToken ct);

    Task DeleteTransactionById(Guid Id, CancellationToken ct);

    Task<bool> UpdateTransaction(Transaction transaction, CancellationToken ct);
}
