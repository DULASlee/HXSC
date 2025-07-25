namespace SmartConstruction.Contracts.Integration;

public class IoTDataDto
{
    public string DeviceCode { get; set; } = null!;
    public DeviceType DeviceType { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

public class IoTCommandDto
{
    public string DeviceCode { get; set; } = null!;
    public string Command { get; set; } = null!;
    public Dictionary<string, object>? Parameters { get; set; }
}