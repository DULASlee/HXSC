using AutoMapper;
using SmartConstruction.Contracts.Dtos.Attendance;
using SmartConstruction.Contracts.Dtos.Company;
using SmartConstruction.Contracts.Dtos.Dashboard;
using SmartConstruction.Contracts.Dtos.Device;
using SmartConstruction.Contracts.Dtos.Integration;
using SmartConstruction.Contracts.Dtos.Project;
using SmartConstruction.Contracts.Dtos.Safety;
using SmartConstruction.Contracts.Dtos.Team;
using SmartConstruction.Contracts.Dtos.Worker;
using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Mappings
{
    /// <summary>
    /// AutoMapper配置
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // 公司映射
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.ProjectCount, opt => opt.MapFrom(src => src.Projects.Count));
            CreateMap<CompanyDto, Company>();
            CreateMap<CreateCompanyRequest, Company>();
            CreateMap<UpdateCompanyRequest, Company>();

            // 项目映射
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.TeamCount, opt => opt.MapFrom(src => src.Teams.Count))
                .ForMember(dest => dest.WorkerCount, opt => opt.MapFrom(src => src.Workers.Count))
                .ForMember(dest => dest.DeviceCount, opt => opt.MapFrom(src => src.Devices.Count));
            CreateMap<ProjectDto, Project>();
            CreateMap<CreateProjectRequest, Project>();
            CreateMap<UpdateProjectRequest, Project>();

            // 班组映射
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : null))
                .ForMember(dest => dest.TeamLeaderName, opt => opt.MapFrom(src => src.TeamLeader != null ? src.TeamLeader.DisplayName : null));
            CreateMap<TeamDto, Team>();
            CreateMap<CreateTeamRequest, Team>();
            CreateMap<UpdateTeamRequest, Team>();

            // 工人映射
            CreateMap<Worker, WorkerDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : null))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.AttendanceProfile != null && src.AttendanceProfile.IsVerified))
                .ForMember(dest => dest.FaceImage, opt => opt.MapFrom(src => src.AttendanceProfile != null ? src.AttendanceProfile.FaceImage : null))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.HasValue ? DateTime.Now.Year - src.BirthDate.Value.Year : (int?)null));
            CreateMap<WorkerDto, Worker>();
            CreateMap<CreateWorkerRequest, Worker>();
            CreateMap<UpdateWorkerRequest, Worker>();

            // 工人实名制考勤资料映射
            CreateMap<WorkerAttendanceProfile, WorkerAttendanceProfile>();

            // 考勤记录映射
            CreateMap<AttendanceRecord, AttendanceDto>()
                .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.DisplayName : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : null))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
                .ForMember(dest => dest.WorkHours, opt => opt.MapFrom(src => src.ClockInTime.HasValue && src.ClockOutTime.HasValue ? (src.ClockOutTime.Value - src.ClockInTime.Value).TotalHours : (double?)null));
            CreateMap<AttendanceDto, AttendanceRecord>();
            CreateMap<CreateAttendanceRequest, AttendanceRecord>();
            CreateMap<UpdateAttendanceRequest, AttendanceRecord>();

            // 设备映射
            CreateMap<Device, DeviceDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName));
            CreateMap<DeviceDto, Device>();
            CreateMap<CreateDeviceRequest, Device>();
            CreateMap<UpdateDeviceRequest, Device>();

            // 安全事故映射
            CreateMap<SafetyIncident, SafetyIncidentDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName));
            CreateMap<SafetyIncidentDto, SafetyIncident>();
            CreateMap<CreateSafetyIncidentRequest, SafetyIncident>();
            CreateMap<UpdateSafetyIncidentRequest, SafetyIncident>();

            // IoT数据映射
            CreateMap<IoTDataDto, IoTRealtimeDataViewModel>();
            CreateMap<IoTAlertDto, IoTAlertViewModel>();
        }
    }
} 