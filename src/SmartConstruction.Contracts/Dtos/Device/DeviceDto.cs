namespace SmartConstruction.Contracts.Dtos.Device;

public class DeviceDto : BaseDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DeviceType Type { get; set; }
    public string TypeName => Type.ToString();
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? InstallDate { get; set; }
    public DeviceStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

public class CreateDeviceDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DeviceType Type { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? InstallDate { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

public class DeviceListRequest : PagedRequestBase
{
    public Guid? ProjectId { get; set; }
    public DeviceType? Type { get; set; }
    public DeviceStatus? Status { get; set; }
}