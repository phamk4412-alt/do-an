using System.Globalization;
using HoChiMinhCrimeSystem;

const string DataFile = "crime_data.json";

var service = new CrimeManagementService(DataFile);
service.LoadData();

while (true)
{
    Console.Clear();
    Console.WriteLine("=== HỆ THỐNG QUẢN LÝ VÀ PHÂN TÍCH DỮ LIỆU TỘI PHẠM - TP. HỒ CHÍ MINH ===");
    Console.WriteLine("1. Thêm vụ án");
    Console.WriteLine("2. Hiển thị danh sách vụ án");
    Console.WriteLine("3. Tìm kiếm vụ án");
    Console.WriteLine("4. Xóa vụ án");
    Console.WriteLine("5. Phân tích dữ liệu");
    Console.WriteLine("0. Thoát");
    Console.Write("Lựa chọn của bạn: ");

    var choice = ReadLineOrExit();
    switch (choice)
    {
        case "1": AddIncident(service); break;
        case "2": ShowAllIncidents(service); break;
        case "3": SearchIncidents(service); break;
        case "4": DeleteIncident(service); break;
        case "5": ShowAnalysis(service); break;
        case "0": service.SaveData(); return;
        default: Console.WriteLine("Lựa chọn không hợp lệ. Nhấn Enter để thử lại..."); ReadLineOrExit(); break;
    }
}

static void AddIncident(CrimeManagementService service)
{
    Console.Clear();
    Console.WriteLine("--- Thêm vụ án mới ---");

    var date = ReadDate("Ngày xảy ra (yyyy-MM-dd): ");
    Console.Write("Quận/huyện: ");
    var district = ReadLineOrExit().Trim();
    Console.Write("Phường/xã: ");
    var ward = ReadLineOrExit().Trim();
    Console.Write("Loại tội phạm: ");
    var type = ReadLineOrExit().Trim();
    Console.Write("Mức độ nghiêm trọng: ");
    var severity = ReadLineOrExit().Trim();
    Console.Write("Trạng thái: ");
    var status = ReadLineOrExit().Trim();
    Console.Write("Mô tả: ");
    var description = ReadLineOrExit().Trim();

    var incident = new CrimeIncident
    (
        Guid.NewGuid(),
        date,
        district,
        ward,
        type,
        severity,
        status,
        description
    );

    service.AddIncident(incident);
    service.SaveData();

    Console.WriteLine("Vụ án đã được thêm.");
    Console.WriteLine("Nhấn Enter để quay lại menu...");
    ReadLineOrExit();
}

static void ShowAllIncidents(CrimeManagementService service)
{
    Console.Clear();
    Console.WriteLine("--- Danh sách vụ án ---");
    var incidents = service.GetAllIncidents();
    if (!incidents.Any())
    {
        Console.WriteLine("Không có dữ liệu vụ án.");
    }
    else
    {
        foreach (var incident in incidents)
        {
            Console.WriteLine(incident.ToDisplayString());
            Console.WriteLine(new string('-', 80));
        }
    }

    Console.WriteLine("Nhấn Enter để quay lại menu...");
    ReadLineOrExit();
}

static void SearchIncidents(CrimeManagementService service)
{
    Console.Clear();
    Console.WriteLine("--- Tìm kiếm vụ án ---");
    Console.WriteLine("1. Theo quận/huyện");
    Console.WriteLine("2. Theo loại tội phạm");
    Console.WriteLine("3. Theo khoảng thời gian");
    Console.WriteLine("0. Quay lại");
    Console.Write("Lựa chọn: ");

    var choice = ReadLineOrExit();
    IEnumerable<CrimeIncident> results = Array.Empty<CrimeIncident>();

    switch (choice)
    {
        case "1":
            Console.Write("Quận/huyện: ");
            var district = ReadLineOrExit().Trim();
            results = service.SearchByDistrict(district);
            break;
        case "2":
            Console.Write("Loại tội phạm: ");
            var type = ReadLineOrExit().Trim();
            results = service.SearchByType(type);
            break;
        case "3":
            var from = ReadDate("Từ ngày (yyyy-MM-dd): ");
            var to = ReadDate("Đến ngày (yyyy-MM-dd): ");
            results = service.SearchByDateRange(from, to);
            break;
        default:
            return;
    }

    Console.WriteLine($"Kết quả tìm kiếm: {results.Count()} vụ án");
    foreach (var incident in results)
    {
        Console.WriteLine(incident.ToDisplayString());
        Console.WriteLine(new string('-', 80));
    }

    Console.WriteLine("Nhấn Enter để quay lại menu...");
    ReadLineOrExit();
}

static void DeleteIncident(CrimeManagementService service)
{
    Console.Clear();
    Console.WriteLine("--- Xóa vụ án ---");
    Console.Write("Nhập ID vụ án: ");
    var idText = ReadLineOrExit();

    if (Guid.TryParse(idText, out var id) && service.DeleteIncident(id))
    {
        service.SaveData();
        Console.WriteLine("Xóa vụ án thành công.");
    }
    else
    {
        Console.WriteLine("Không tìm thấy vụ án với ID đã nhập.");
    }

    Console.WriteLine("Nhấn Enter để quay lại menu...");
    ReadLineOrExit();
}

static void ShowAnalysis(CrimeManagementService service)
{
    Console.Clear();
    Console.WriteLine("--- Phân tích dữ liệu tội phạm ---");
    var summary = service.GenerateSummary();

    Console.WriteLine($"Tổng số vụ án: {summary.TotalIncidents}");
    Console.WriteLine("Số vụ theo quận/huyện:");
    foreach (var group in summary.ByDistrict)
    {
        Console.WriteLine($"  {group.Key}: {group.Value}");
    }

    Console.WriteLine("Số vụ theo loại tội phạm:");
    foreach (var group in summary.ByType)
    {
        Console.WriteLine($"  {group.Key}: {group.Value}");
    }

    Console.WriteLine("Số vụ theo tháng:");
    foreach (var group in summary.ByMonth)
    {
        Console.WriteLine($"  {group.Key:yyyy-MM}: {group.Value}");
    }

    Console.WriteLine($"Loại tội phạm phổ biến nhất: {summary.MostFrequentType}");
    Console.WriteLine($"Quận/huyện có nhiều vụ nhất: {summary.MostActiveDistrict}");

    Console.WriteLine("Nhấn Enter để quay lại menu...");
    ReadLineOrExit();
}

static DateTime ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var input = ReadLineOrExit();
        if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        Console.WriteLine("Định dạng không hợp lệ. Vui lòng nhập lại theo mẫu yyyy-MM-dd.");
    }
}

static string ReadLineOrExit()
{
    var input = Console.ReadLine();
    if (input is null)
    {
        Console.WriteLine("Không nhận được dữ liệu đầu vào. Ứng dụng sẽ thoát.");
        Environment.Exit(0);
    }
    return input;
}

