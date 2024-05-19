using AksiaSoftwareDeveloper.DataAccess.Interfaces;

namespace AksiaSoftwareDeveloper.DataAccess.DBModels;

public class DBEntity : IDBEntity
{
    public required Guid Id { get; set; }
}
