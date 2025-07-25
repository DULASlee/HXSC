# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is SmartConstruction (智慧工地管理系统) - a comprehensive smart construction management system built with .NET 8. It's an enterprise-grade platform for managing construction projects, companies, teams, workers, equipment, and government compliance.

## Development Commands

### Running the Application
```bash
# Start the API service (backend)
cd src/SmartConstruction.Service
dotnet run  # Runs on http://localhost:8998

# Start the web application (frontend) - Vue.js
cd src/SmartConstruction.Web
npm run dev  # Runs on http://localhost:5173

# Build entire solution (backend)
dotnet build SmartConstruction.sln

# Restore .NET packages
dotnet restore SmartConstruction.sln

# Install frontend dependencies
cd src/SmartConstruction.Web
npm install

# Build frontend for production
npm run build

# Lint frontend code
npm run lint

# Format frontend code
npm run format

# Generate API types from OpenAPI (ensure backend is running)
npm run api:generate  # Uses http://localhost:7999/swagger/v1/swagger.json
```

### Development Environment
- **API**: http://localhost:8998 (Swagger: /swagger)
- **Web App**: http://localhost:5173 (Vue.js dev server)
- **Database**: SQL Server Express with two databases (ApplicationDbContext, SmartConstructionDbContext for IoT)
- **SignalR Hub**: http://localhost:8998/hubs/iotdata
- **Note**: API generation script uses port 7999 - update this to match actual backend port 8998

## Architecture Overview

### Clean Architecture Structure
The project follows strict Clean Architecture with three main layers:

1. **SmartConstruction.Contracts** (Domain Layer)
   - Entities: Core business objects (Company, Project, Team, Worker, etc.)
   - DTOs: Data transfer objects for API communication
   - Enums: Shared enumerations

2. **SmartConstruction.Service** (API/Infrastructure Layer)
   - Controllers: RESTful API endpoints
   - Services: Business logic implementation
   - Infrastructure: DbContext, repositories, UnitOfWork pattern
   - Hubs: SignalR real-time communication

3. **SmartConstruction.Web** (Presentation Layer)
   - Vue.js 3 application with TypeScript
   - Element Plus UI components
   - State management with Pinia
   - API service clients using Axios

### Key Architectural Patterns
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **CQRS-like structure**: Separate command/query handling
- **Dependency Injection**: ASP.NET Core DI container
- **AutoMapper**: Object-to-object mapping

## Technology Stack

### Backend
- ASP.NET Core 8 with Entity Framework Core
- SQL Server (ApplicationDbContext for main data, SmartConstructionDbContext for IoT)
- JWT Bearer authentication with refresh tokens
- SignalR for real-time IoT data and updates
- AutoMapper for object mapping
- Swagger/OpenAPI documentation
- Serilog for structured logging (Warning+ level, JSON format, E:/SmartCLog directory)
- Comprehensive API audit and testing framework
- Database initialization service for IoT data models

### Frontend
- Vue.js 3.3.11 with TypeScript 5.4.0 and Composition API
- Vite 5.0.7 build tool with advanced optimization
- Element Plus 2.4.4 UI component library
- Pinia 2.1.7 for state management (Vue's official store)
- Axios for HTTP API calls with proxy to backend (/api → localhost:8998)
- ECharts 5.4.3 for data visualization and dashboards
- Vue I18n for internationalization (Chinese/English)
- Vue Router for navigation with role-based guards
- Comprehensive digital twin visualization system

### Infrastructure
- Redis caching
- MQTT for IoT device integration
- Serilog for logging
- FluentValidation for input validation

## Key Business Domains

### Core Modules
- **Multi-tenancy**: Tenant isolation and management
- **Company Management**: Construction company registration
- **Project Management**: Full project lifecycle
- **Team & Worker Management**: Personnel and attendance tracking
- **Device Management**: Tower crane and elevator monitoring
- **Safety Management**: Incident reporting and compliance
- **Government Compliance**: Automated reporting to authorities

### Special Features
- **IoT Integration**: Real-time device data collection via MQTT
- **Digital Twin System**: 5 comprehensive monitoring screens (Command Center, Attendance, Video Monitor, Crane/Elevator Management, Environment Monitoring)
- **3D Scene Visualization**: BIM model integration with real-time device positioning
- **Dynamic Forms**: Component-based form generation with validation
- **Face Recognition**: Attendance tracking with biometric validation
- **Real-time Dashboards**: Live data visualization with WebSocket updates
- **Multi-language Support**: Chinese/English interface
- **Heat Map Analytics**: Personnel density, equipment utilization, and safety risk visualization

## Database Context

### Primary Entities
- Company, Project, Team, Worker
- AttendanceRecord, Device, SafetyIncident
- GovernmentReport, Tenant

### Key Relationships
- Companies contain multiple Projects
- Projects have Teams with Workers
- AttendanceRecords track Worker presence
- Devices monitor construction equipment
- Multi-tenant data isolation

## Configuration Notes

### Connection Strings
- Uses SQL Server with dual database architecture
- ApplicationDbContext: Main application data (users, companies, projects, teams, workers)
- SmartConstructionDbContext: IoT and real-time data (devices, sensors, digital twin models)
- Connection strings configured in appsettings.json
- Database initialization service with comprehensive test data generation

### Authentication
- JWT Bearer tokens
- OpenID Connect support
- Role-based authorization

### External Integrations
- Government API reporting
- MQTT broker for IoT devices
- Redis for distributed caching
- Email/SMS notification services

## Code Quality Rules

The project enforces strict architectural rules via Cursor configuration (.cursor/rules.json):

### Layer Separation
- Domain layer (Contracts) cannot reference other layers
- API layer cannot directly reference Web layer  
- Each layer has specific responsibilities
- Files must be placed in correct architectural layer directories

### File Organization
- **Domain Layer**: Entities, ValueObjects, Aggregates, Events only
- **Application Layer**: Services, UseCases, Queries, Commands, DTOs
- **Infrastructure Layer**: Repositories, External Services, MQTT/TCP integrations
- **API Layer**: Controllers, Hubs, Program.cs configuration
- **Web Layer**: Vue components, stores, services
- Strict directory structure enforcement with automatic validation

### Code Standards  
- **No duplicate type names** across layers (enforced automatically)
- **No mock/example/todo code** allowed in production files
- **File deletion confirmation** required for safety
- **Consistent naming patterns** for DTOs, entities, and services
- **Proper namespace organization** following project structure
- **Layer-specific imports** - each layer can only import from appropriate layers

### Frontend Standards
- **TypeScript strict mode** enabled with full type checking
- **ESLint and Prettier** for automated code formatting
- **Component-based architecture** with Element Plus integration
- **Consistent Vue 3 Composition API** usage with `<script setup>` syntax
- **Performance optimization** through Vite build configuration

## Important Notes

- **Quality Assurance Framework**: Includes API audit service, automated testing framework, and data integrity validation
- **Digital Twin System**: Complete implementation with 25+ specialized APIs for real-time construction monitoring
- **Real-time Features**: Extensive use of SignalR for live updates with IoTDataHub
- **Government Compliance**: Critical feature requiring careful handling
- **Multi-language**: Interface supports Chinese localization
- **Mobile Responsive**: Web app designed for mobile construction site use
- **Structured Logging**: Serilog with Warning+ level filtering, JSON format, daily rotation

## Digital Twin System Architecture

The system features a comprehensive digital twin implementation with 5 main monitoring screens:

### 1. Command Center Screen (指挥中心大屏)
- **API Endpoints**: `/api/digital-twin/command-center/*`
- **Features**: Project overview, personnel statistics, equipment status, safety metrics
- **Real-time Updates**: 30-second refresh intervals
- **Key Components**: Project summary cards, real-time statistics, trend analysis

### 2. Attendance Monitoring Screen (项目考勤大屏)
- **API Endpoints**: `/api/digital-twin/attendance/*`
- **Features**: Daily attendance overview, team rankings, real-time check-ins
- **Real-time Updates**: 10-second refresh intervals
- **Key Components**: Attendance statistics, team performance, hourly distribution

### 3. Video Surveillance Screen (视频监控大屏)
- **API Endpoints**: `/api/digital-twin/video-monitor/*`
- **Features**: Camera management, AI analysis, recording statistics
- **Real-time Updates**: 5-second refresh intervals
- **Key Components**: 9-grid video display, AI detection results, storage management

### 4. Crane & Elevator Management Screen (塔吊升降机管理大屏)
- **API Endpoints**: `/api/digital-twin/crane-elevator/*`
- **Features**: Equipment real-time data, safety monitoring, efficiency analysis
- **Real-time Updates**: 15-second refresh intervals
- **Key Components**: Device status cards, safety alerts, performance metrics

### 5. Environment Monitoring Screen (扬尘噪音监测大屏)
- **API Endpoints**: `/api/digital-twin/environment/*`
- **Features**: Air quality monitoring, noise levels, weather data
- **Real-time Updates**: 60-second refresh intervals
- **Key Components**: PM2.5/PM10 readings, noise monitoring, environmental trends

### Advanced Digital Twin Features

#### 3D Scene Visualization
- **API**: `/api/digital-twin/3d-scene-model`
- **BIM Integration**: `/api/digital-twin/bim-model-data`
- **Real-time Positioning**: `/api/digital-twin/device-positions`
- **Features**: Building models, equipment animation, environmental rendering

#### Data Analytics
- **Heat Maps**: `/api/digital-twin/heatmap-data` (personnel density, equipment utilization, safety risks)
- **Real-time Streams**: `/api/digital-twin/realtime-data-stream`
- **Test Data Generation**: `/api/digital-twin/generate-test-data`
- **Statistics**: `/api/digital-twin/data-statistics`

### Digital Twin Service Architecture
- **Interface**: `IDigitalTwinService` with 25+ specialized methods
- **Implementation**: `DigitalTwinService` with comprehensive mock data generation
- **Controller**: `DigitalTwinController` with full API endpoints
- **Real-time Hub**: `IoTDataHub` for SignalR communication
- **Database Models**: IoT entities in `SmartConstructionDbContext`

## Common Patterns

### API Development
- Controllers inherit from ControllerBase
- Use AutoMapper for DTO mapping
- Implement proper error handling
- Follow RESTful conventions

### Vue.js Development
- Components use Element Plus UI library
- State management via Pinia stores
- API calls through Axios with TypeScript
- Vue Router for client-side routing
- Composition API with `<script setup>` syntax
- TypeScript for type safety

### Data Access
- Use UnitOfWork pattern for transactions
- Repository pattern for data access
- Entity Framework Core for ORM
- Proper async/await patterns
- Dual database context pattern (Application + IoT)
- Database initialization with comprehensive test data generation

## Development Workflow

### Proxy Configuration
- Frontend development server proxies `/api` requests to `http://localhost:8998`
- CORS configured on backend to allow `http://localhost:3000`
- SignalR hubs accessible at `/hubs/*` endpoints

### Database Development
- Two separate database contexts for logical separation:
  - `ApplicationDbContext`: Main application data (tenants, users, companies, projects, teams, workers, attendance, safety)
  - `SmartConstructionDbContext`: IoT device data, sensors, real-time metrics, digital twin models, BIM data
- Entity Framework migrations manage schema changes
- DatabaseInitializationService provides comprehensive test data generation
- Supports data cleanup and integrity validation

### Development Tools and Testing
- **ApiAuditService**: Interface completeness checking and repair suggestions
- **ApiTestFramework**: Self-healing automated testing with coverage reporting
- **DatabaseInitializationService**: Comprehensive test data generation for IoT scenarios
- **LoggingConfiguration**: Structured logging (Warning+ level, JSON format, E:/SmartCLog)
- **Digital Twin Mock Services**: Complete test data for all 5 monitoring screens
- **Windows Compatibility**: Startup scripts for WSL2 environments (start-windows.bat, start-windows.ps1)
- **Quality Assurance Tools**: 
  - Data integrity validation
  - API endpoint discovery
  - Automated repair suggestions
  - Coverage analysis and reporting
  - Self-healing test capabilities

## Cursor Rules Integration

This project enforces strict development rules via Cursor configuration:

### Project Constraints (from .cursor/cursor.md)
1. **No Duplicate Code**: Cannot create duplicate variables, methods, or classes
2. **No File Deletion**: Only creation, editing, and commenting allowed
3. **Layer Separation**: Strict adherence to Clean Architecture layers
4. **Compilation Requirement**: Must fix all compilation errors before completion
5. **No Intermediate Questions**: Direct execution without confirmation requests

### Directory Structure Rules
| Layer | Allowed Content |
|-------|----------------|
| Domain/ | Entities, Value Objects, Domain Events |
| Application/ | CQRS Commands/Queries, DTOs, Interfaces |
| Infrastructure/ | Repository implementations, MQTT/TCP gateways, EF configurations |
| Api/ | Controllers, Hubs, Program.cs |
| Web/ | Blazor pages, components, services |
| Tests/ | Unit/Integration tests |

### Code Quality Standards
- All public methods require XML documentation
- All repository methods must be async
- All DTOs must end with "Dto" suffix
- All enums must be in Enums/ directory
- Use ILogger<T> for logging
- Wrap exceptions as BusinessException
- No mock/example/todo/fixme code in production

### Memory-Aware Development
- Accept only atomic-level tasks (<128 tokens)
- Complete one task at a time with human confirmation
- Avoid generating entire codebases in single operations
- Use trigger words: @plan, @clean, @guard for specific actions

# important-instruction-reminders
Do what has been asked; nothing more, nothing less.
NEVER create files unless they're absolutely necessary for achieving your goal.
ALWAYS prefer editing an existing file to creating a new one.
NEVER proactively create documentation files (*.md) or README files. Only create documentation files if explicitly requested by the User.