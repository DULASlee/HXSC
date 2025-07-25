using SmartConstruction.Contracts.Dtos.ConfigChange;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IConfigChangeService : IBaseService<ConfigChange, ConfigChangeDto, CreateConfigChangeRequest, UpdateConfigChangeRequest>
{
} 