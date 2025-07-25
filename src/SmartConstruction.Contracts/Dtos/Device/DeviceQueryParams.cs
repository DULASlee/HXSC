namespace SmartConstruction.Contracts.Dtos.Device;

public class DeviceQueryParams
{
    public string? DeviceCode { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Status { get; set; }
    public Guid? ProjectId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
