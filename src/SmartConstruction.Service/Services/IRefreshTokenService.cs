using SmartConstruction.Contracts.Dtos.RefreshToken;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IRefreshTokenService : IBaseService<RefreshToken, RefreshTokenDto, CreateRefreshTokenRequest, UpdateRefreshTokenRequest>
{
} 