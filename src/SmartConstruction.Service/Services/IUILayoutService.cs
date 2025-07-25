using SmartConstruction.Contracts.Dtos.UILayout;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IUILayoutService : IBaseService<UILayout, UILayoutDto, CreateUILayoutRequest, UpdateUILayoutRequest>
{
} 