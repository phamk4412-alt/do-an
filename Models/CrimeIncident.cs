namespace HoChiMinhCrimeSystem;

public sealed record CrimeIncident(
    Guid Id,
    DateTime Date,
    string District,
    string Ward,
    string Type,
    string Severity,
    string Status,
    string Description)
{
    public string ToDisplayString() =>
        $"ID: {Id}\nNgày: {Date:yyyy-MM-dd}\nQuận/huyện: {District}\nPhường/xã: {Ward}\nLoại: {Type}\nMức độ: {Severity}\nTrạng thái: {Status}\nMô tả: {Description}";
}
