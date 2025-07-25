using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.AuditLog;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class AuditLogService : BaseService<AuditLog, AuditLogDto, CreateAuditLogRequest, UpdateAuditLogRequest>, IAuditLogService
{
    public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuditLogService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 