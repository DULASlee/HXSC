using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.RefreshToken;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class RefreshTokenService : BaseService<RefreshToken, RefreshTokenDto, CreateRefreshTokenRequest, UpdateRefreshTokenRequest>, IRefreshTokenService
{
    public RefreshTokenService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RefreshTokenService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 
