namespace HoChiMinhCrimeSystem;

public sealed record CrimeSummary(
    int TotalIncidents,
    IReadOnlyDictionary<string, int> ByDistrict,
    IReadOnlyDictionary<string, int> ByType,
    IReadOnlyDictionary<DateTime, int> ByMonth,
    string MostFrequentType,
    string MostActiveDistrict);
