namespace AksiaSoftwareDeveloper.Common.DTO;

public record TransactionDTO
{
    public required Guid Id { get; set; }
    public required string ApplicationName { get; set; }
    public required string Email { get; set; }
    public string? Filename { get; set; }
    public string? Url { get; set; }
    public DateTime? Inception { get; set; }
    public required string Amount { get; set; }
    public decimal Allocation { get; set; }
}
