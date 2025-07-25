using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Hubs;
using SmartConstruction.Service.Infrastructure.Database;
using SmartConstruction.Service.Infrastructure.Logging;
using SmartConstruction.Service.Infrastructure.Repositories;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Middleware;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks; // 添加 Task 的 using

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. 日志
        builder.Host.UseSerilog(LoggingConfiguration.ConfigureLogging(builder.Configuration, builder.Environment));

        // 2. 服务
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // 数据库上下文
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDbContext<SmartConstructionDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IoTConnection")));

        // 仓储和工作单元
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        builder.Services.AddScoped(typeof(IDigitalTwinRepository<>), typeof(DigitalTwinRepository<>));
        builder.Services.AddScoped<IDigitalTwinUnitOfWork, DigitalTwinUnitOfWork>();

        // 数据初始化服务
        builder.Services.AddScoped<DataSeeder>();
        builder.Services.AddScoped<DigitalTwinDataSeeder>();

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        // JWT
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if(jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
        {
            throw new InvalidOperationException("JWT settings are not configured properly.");
        }
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });

        // 业务服务
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();
        builder.Services.AddScoped<IMenuService, MenuService>();
        builder.Services.AddScoped<ICompanyService, CompanyService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IAttendanceService, AttendanceService>();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<ISafetyIncidentService, SafetyIncidentService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        builder.Services.AddScoped<IDigitalTwinService, DigitalTwinService>();
        builder.Services.AddScoped<LoggingService>(); // 注册 LoggingService

        // SignalR, Swagger, CORS, HealthChecks
        builder.Services.AddSignalR();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartConstruction API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        builder.Services.AddHealthChecks();

        // 3. 中间件管道
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartConstruction API v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TenantPermissionMiddleware>();

        app.MapControllers();
        app.MapHub<IoTDataHub>("/hubs/iot");
        app.MapHealthChecks("/health");

        // 数据库初始化 (注释掉以禁止启动时自动执行)
        /*
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // 应用 ApplicationDbContext 的迁移
                var appDbContext = services.GetRequiredService<ApplicationDbContext>();
                await appDbContext.Database.MigrateAsync();
                
                // 应用 SmartConstructionDbContext 的迁移
                var iotDbContext = services.GetRequiredService<SmartConstructionDbContext>();
                await iotDbContext.Database.MigrateAsync();

                // 执行种子数据填充
                await services.GetRequiredService<DataSeeder>().SeedAsync();
                await services.GetRequiredService<DigitalTwinDataSeeder>().SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "数据库初始化期间发生错误。");
            }
        }
        */

        app.Run();
    }
} 