using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.UILayout;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class UILayoutService : BaseService<UILayout, UILayoutDto, CreateUILayoutRequest, UpdateUILayoutRequest>, IUILayoutService
{
    public UILayoutService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UILayoutService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 