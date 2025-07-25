using SmartConstruction.Contracts.Dtos.AuditLog;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IAuditLogService : IBaseService<AuditLog, AuditLogDto, CreateAuditLogRequest, UpdateAuditLogRequest>
{
} 