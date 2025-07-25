using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.UIComponent;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class UIComponentService : BaseService<UIComponent, UIComponentDto, CreateUIComponentRequest, UpdateUIComponentRequest>, IUIComponentService
{
    public UIComponentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UIComponentService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 