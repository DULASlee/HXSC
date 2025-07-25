using SmartConstruction.Contracts.Dtos.UIComponent;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IUIComponentService : IBaseService<UIComponent, UIComponentDto, CreateUIComponentRequest, UpdateUIComponentRequest>
{
} 