using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.ConfigChange;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class ConfigChangeService : BaseService<ConfigChange, ConfigChangeDto, CreateConfigChangeRequest, UpdateConfigChangeRequest>, IConfigChangeService
{
    public ConfigChangeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ConfigChangeService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 
