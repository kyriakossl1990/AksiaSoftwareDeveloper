using AksiaSoftwareDeveloper.Common.DTO;
using AksiaSoftwareDeveloper.DataAccess.DBModels;

namespace AksiaSoftwareDeveloper.Core.Mappers;

public static class TransactionMapper
{
    public static TransactionDTO TransactionToTransactionDTOMap(Transaction transaction)
    {
        try
        {
            return new TransactionDTO
            {
                Id = transaction.Id,
                ApplicationName = transaction.ApplicationName,
                Amount = transaction.Amount,
                Email = transaction.Email,
                Allocation = transaction.Allocation,
                Filename = transaction.Filename,
                Inception = transaction.Inception,
                Url = transaction.Url
            };
        }
        catch
        {
            throw new Exception("TransactionToTransactionDTOMap failed");
        }
    }

    public static Transaction TransactionDTOToTransactionMap(TransactionDTO transactionDTO)
    {
        try
        {
            return new Transaction
            {
                Id = transactionDTO.Id,
                ApplicationName = transactionDTO.ApplicationName,
                Amount = transactionDTO.Amount,
                Email = transactionDTO.Email,
                Allocation = transactionDTO.Allocation,
                Filename = transactionDTO.Filename,
                Inception = transactionDTO.Inception,
                Url = transactionDTO.Url
            };
        }
        catch
        {
            throw new Exception("TransactionDTOToTransactionMap failed");
        }
    }

}
