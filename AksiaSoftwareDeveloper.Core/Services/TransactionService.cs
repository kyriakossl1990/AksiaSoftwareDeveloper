using AksiaSoftwareDeveloper.Common;
using AksiaSoftwareDeveloper.Common.DTO;
using AksiaSoftwareDeveloper.Core.Interfaces;
using AksiaSoftwareDeveloper.Core.Mappers;
using AksiaSoftwareDeveloper.DataAccess.DBModels;
using AksiaSoftwareDeveloper.DataAccess.Interfaces;

namespace AksiaSoftwareDeveloper.Core.Services;

public class TransactionService : ITransactionService
{
    readonly string[] AllowedFileTypes = ["png", "mp3", "tiff", "xls", "pdf"];

    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository
            ?? throw new Exception("transactionRepository not found");
    }

    public async Task<PaginatedResponse<TransactionDTO>> GetAllTransactions(int currentPage, int pageSize, CancellationToken ct)
    {
        var list = new List<TransactionDTO>();

        var result = await _transactionRepository.GetAllTransactions(currentPage, pageSize, ct);

        foreach (Transaction transaction in result.Entities)
        {
            list.Add(TransactionMapper.TransactionToTransactionDTOMap(transaction));
        }

        return new PaginatedResponse<TransactionDTO>
        {
            Entities = list,
            TotalCount = result.TotalCount,
            TotalPage = result.TotalPage
        };
    }

    public async Task<TransactionDTO> GetTransactionById(Guid id, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionById(id, ct);
        return TransactionMapper.TransactionToTransactionDTOMap(transaction);
    }

    public async Task UpdateTransaction(TransactionDTO transaction, CancellationToken ct)
    {
        await ValidateTransactionDTOBeforeUpdate(transaction, ct);
        var transactionToUpdate = TransactionMapper.TransactionDTOToTransactionMap(transaction);

        if (!await _transactionRepository.UpdateTransaction(transactionToUpdate, ct))
            throw new Exception($"Transaction update with id:{transactionToUpdate.Id} failed");
    }

    public async Task DeleteTransactionById(Guid Id, CancellationToken ct)
    {
        await _transactionRepository.DeleteTransactionById(Id, ct);
    }

    private async Task ValidateTransactionDTOBeforeUpdate(TransactionDTO transaction, CancellationToken ct)
    {
        var fileType = transaction?.Filename?
            .Split('.')
            .Last()
            .Trim();

        if (!AllowedFileTypes.Contains(fileType))
            throw new Exception($"{fileType} is not allow as filetype");

        if (transaction?.Inception is not null)
        {
            if (transaction?.Inception > DateTime.Now)
                throw new Exception("Inception should be a past date");
        }

        var storedTrans = await _transactionRepository.GetTransactionById(transaction.Id, ct);

        var stroredCurrency = storedTrans.Amount[0];
        var newCurrency = transaction.Amount[0];

        if (!stroredCurrency.Equals(newCurrency))
            throw new Exception("Currency cant be changed");
    }
}
