using AksiaSoftwareDeveloper.Common;
using AksiaSoftwareDeveloper.Common.DTO;

namespace AksiaSoftwareDeveloper.Core.Interfaces;

public interface ITransactionService
{
    Task<PaginatedResponse<TransactionDTO>> GetAllTransactions(int currentPage, int pageSize, CancellationToken ct);

    Task<TransactionDTO> GetTransactionById(Guid Id, CancellationToken ct);

    Task DeleteTransactionById(Guid Id, CancellationToken ct);

    Task UpdateTransaction(TransactionDTO transaction, CancellationToken ct);
}
