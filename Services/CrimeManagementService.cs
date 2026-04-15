using System.Text.Json;

namespace HoChiMinhCrimeSystem;

public sealed class CrimeManagementService
{
    private readonly string _dataFile;
    private readonly List<CrimeIncident> _incidents = new();

    public CrimeManagementService(string dataFile)
    {
        _dataFile = dataFile;
    }

    public void LoadData()
    {
        if (!File.Exists(_dataFile))
        {
            return;
        }

        var json = File.ReadAllText(_dataFile);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<CrimeIncident>>(json, options);
        if (list != null)
        {
            _incidents.Clear();
            _incidents.AddRange(list);
        }
    }

    public void SaveData()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_incidents, options);
        File.WriteAllText(_dataFile, json);
    }

    public void AddIncident(CrimeIncident incident) => _incidents.Add(incident);
    public IReadOnlyCollection<CrimeIncident> GetAllIncidents() => _incidents.AsReadOnly();
    public bool DeleteIncident(Guid id) => _incidents.RemoveAll(i => i.Id == id) > 0;
    public IEnumerable<CrimeIncident> SearchByDistrict(string district) =>
        _incidents.Where(i => i.District.Contains(district, StringComparison.OrdinalIgnoreCase));
    public IEnumerable<CrimeIncident> SearchByType(string type) =>
        _incidents.Where(i => i.Type.Contains(type, StringComparison.OrdinalIgnoreCase));
    public IEnumerable<CrimeIncident> SearchByDateRange(DateTime from, DateTime to) =>
        _incidents.Where(i => i.Date.Date >= from.Date && i.Date.Date <= to.Date);

    public CrimeSummary GenerateSummary()
    {
        var byDistrict = _incidents
            .GroupBy(i => string.IsNullOrWhiteSpace(i.District) ? "Chưa xác định" : i.District)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        var byType = _incidents
            .GroupBy(i => string.IsNullOrWhiteSpace(i.Type) ? "Chưa xác định" : i.Type)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        var byMonth = _incidents
            .GroupBy(i => new DateTime(i.Date.Year, i.Date.Month, 1))
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        var mostFrequentType = byType.Keys.FirstOrDefault() ?? "Không có dữ liệu";
        var mostActiveDistrict = byDistrict.Keys.FirstOrDefault() ?? "Không có dữ liệu";

        return new CrimeSummary(_incidents.Count, byDistrict, byType, byMonth, mostFrequentType, mostActiveDistrict);
    }
}
