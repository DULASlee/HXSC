# 🏗️ 智能建造管理系统 (Smart Construction Management System)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.0-green.svg)](https://vuejs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue.svg)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> 基于.NET 8 + Vue 3 + TypeScript的现代化智能建造管理平台，集成数字孪生技术，提供全方位的工地管理解决方案。

## 📋 目录

- [项目概述](#项目概述)
- [核心功能](#核心功能)
- [技术架构](#技术架构)
- [项目结构](#项目结构)
- [快速开始](#快速开始)
- [API文档](#api文档)
- [部署指南](#部署指南)
- [开发指南](#开发指南)
- [贡献指南](#贡献指南)
- [许可证](#许可证)

---

## 🎯 项目概述

智能建造管理系统是一个集成了现代信息技术的综合性工地管理平台，旨在通过数字化手段提升建筑工地的管理效率、安全性和智能化水平。

### 🌟 主要特色

- **🔐 多租户架构**：支持多企业、多项目独立管理
- **👥 人员管理**：工人信息、考勤管理、技能评估
- **📱 设备监控**：IoT设备集成、实时状态监控
- **🚨 安全管理**：安全事故处理、安全培训记录
- **📊 数字孪生**：3D可视化、实时数据展示
- **📈 数据分析**：多维度统计、趋势分析
- **🔔 智能告警**：实时监控、自动告警

---

## 🚀 核心功能

### 👤 用户权限管理
- **多级权限控制**：基于RBAC的权限管理
- **JWT认证**：安全的令牌认证机制
- **多租户隔离**：租户级别的数据隔离
- **菜单权限**：动态菜单和功能权限控制

### 🏢 企业管理
- **公司信息管理**：企业基本信息、资质管理
- **项目管理**：项目全生命周期管理
- **团队管理**：施工团队组织和人员分配
- **合同管理**：项目合同和协议管理

### 👷 工人管理
- **工人档案**：完整的工人信息管理
- **考勤系统**：人脸识别考勤、工时统计
- **技能评估**：工种分类、技能等级管理
- **培训记录**：安全培训、技能培训记录

### 📱 设备管理
- **设备档案**：设备信息、维护记录
- **IoT集成**：传感器、摄像头等设备接入
- **实时监控**：设备状态、数据采集
- **预警系统**：设备故障、异常告警

### 🚨 安全管理
- **安全事故管理**：事故记录、处理流程
- **安全检查**：定期检查、隐患整改
- **安全培训**：培训计划、考核记录
- **风险评估**：风险识别、评估报告

### 📊 数字孪生大屏
- **3D可视化**：工地3D模型展示
- **实时监控**：人员、设备、环境实时数据
- **数据统计**：多维度数据分析和展示
- **智能告警**：异常情况自动告警

### 📈 数据分析
- **考勤分析**：出勤率、工时统计
- **安全分析**：事故趋势、风险评估
- **设备分析**：设备利用率、故障分析
- **项目分析**：进度跟踪、成本分析

---

## 🏗️ 技术架构

### 后端技术栈
- **.NET 8**：核心框架
- **Entity Framework Core**：ORM框架
- **ASP.NET Core Web API**：RESTful API
- **JWT Authentication**：身份认证
- **AutoMapper**：对象映射
- **Serilog**：日志记录
- **SignalR**：实时通信

### 前端技术栈
- **Vue 3**：前端框架
- **TypeScript**：类型安全
- **Vite**：构建工具
- **Element Plus**：UI组件库
- **Pinia**：状态管理
- **Vue Router**：路由管理
- **ECharts**：数据可视化

### 数据库
- **SQL Server**：主数据库
- **Redis**：缓存和会话存储
- **MongoDB**：日志和文档存储

### 部署和运维
- **Docker**：容器化部署
- **Nginx**：反向代理
- **Jenkins**：CI/CD流水线

---

## 📁 项目结构

```
shangshizhi/
├── src/
│   ├── SmartConstruction.Contracts/     # 共享契约层
│   │   ├── Dtos/                       # 数据传输对象
│   │   ├── Entities/                   # 实体模型
│   │   └── Enums/                      # 枚举定义
│   │
│   ├── SmartConstruction.Service/      # 后端服务层
│   │   ├── Controllers/                # API控制器
│   │   ├── Services/                   # 业务服务
│   │   ├── Infrastructure/             # 基础设施
│   │   ├── Models/                     # 响应模型
│   │   └── Hubs/                       # SignalR集线器
│   │
│   ├── SmartConstruction.Web/          # 前端应用
│   │   ├── src/
│   │   │   ├── api/                    # API接口
│   │   │   ├── components/             # Vue组件
│   │   │   ├── views/                  # 页面视图
│   │   │   ├── stores/                 # 状态管理
│   │   │   ├── router/                 # 路由配置
│   │   │   └── utils/                  # 工具函数
│   │   └── public/                     # 静态资源
│   │
│   └── SmartConstruction.Shared/       # 共享组件
│
├── docs/                               # 项目文档
│   ├── 后端API对接文档.md              # API接口文档
│   ├── 项目管理API文档.md              # 项目管理接口
│   ├── 设备管理API文档.md              # 设备管理接口
│   └── 数字孪生大屏API接口规范.md      # 大屏接口规范
│
└── README.md                           # 项目说明
```

---

## 🚀 快速开始

### 环境要求

- **.NET 8 SDK**
- **Node.js 18+**
- **SQL Server 2019+**
- **Redis 6.0+**

### 后端启动

```bash
# 进入后端目录
cd src/SmartConstruction.Service

# 还原依赖
dotnet restore

# 运行数据库迁移
dotnet ef database update

# 启动服务
dotnet run
```

### 前端启动

```bash
# 进入前端目录
cd src/SmartConstruction.Web

# 安装依赖
npm install

# 启动开发服务器
npm run dev
```

### 数据库配置

1. 创建SQL Server数据库
2. 修改 `appsettings.json` 中的连接字符串
3. 运行数据库迁移

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartConstruction;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

---

## 📚 API文档

### 核心接口文档

- **[认证与权限管理](./docs/后端API对接文档.md)** - 用户登录、权限控制
- **[项目管理接口](./docs/项目管理API文档.md)** - 项目、公司、团队管理
- **[设备管理接口](./docs/设备管理API文档.md)** - 设备监控、IoT集成
- **[数字孪生大屏接口](./docs/数字孪生大屏API接口规范.md)** - 大屏数据接口

### 接口规范

- **基础URL**: `http://localhost:5000/api`
- **认证方式**: Bearer Token (JWT)
- **数据格式**: JSON
- **字符编码**: UTF-8

### 示例请求

```bash
# 用户登录
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "123456",
    "tenantCode": "default"
  }'

# 获取用户信息
curl -X GET http://localhost:5000/api/auth/user-info \
  -H "Authorization: Bearer {token}"
```

---

## 🐳 部署指南

### Docker部署

```bash
# 构建镜像
docker build -t smart-construction .

# 运行容器
docker run -d -p 5000:5000 --name smart-construction smart-construction
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: ./src/SmartConstruction.Service
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - database
      - redis

  web:
    build: ./src/SmartConstruction.Web
    ports:
      - "80:80"
    depends_on:
      - api

  database:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"

  redis:
    image: redis:6-alpine
    ports:
      - "6379:6379"
```

### 生产环境配置

1. **环境变量配置**
2. **数据库连接优化**
3. **缓存策略配置**
4. **日志级别调整**
5. **安全配置加固**

---

## 👨‍💻 开发指南

### 开发环境搭建

1. **克隆项目**
```bash
git clone https://github.com/your-username/smart-construction.git
cd smart-construction
```

2. **后端开发**
```bash
cd src/SmartConstruction.Service
dotnet restore
dotnet run
```

3. **前端开发**
```bash
cd src/SmartConstruction.Web
npm install
npm run dev
```

### 代码规范

- **C#代码规范**：遵循Microsoft C#编码约定
- **TypeScript规范**：使用ESLint + Prettier
- **Git提交规范**：使用Conventional Commits
- **API设计规范**：遵循RESTful API设计原则

### 测试

```bash
# 后端测试
dotnet test

# 前端测试
npm run test

# E2E测试
npm run test:e2e
```

---

## 🤝 贡献指南

我们欢迎所有形式的贡献！

### 贡献方式

1. **报告Bug**：在Issues中报告问题
2. **功能建议**：提出新功能建议
3. **代码贡献**：提交Pull Request
4. **文档改进**：完善项目文档

### 开发流程

1. Fork项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建Pull Request

### 代码审查

- 所有代码变更都需要通过Pull Request
- 至少需要一名维护者审查
- 确保代码质量和测试覆盖率

---

## 📄 许可证

本项目采用 [MIT许可证](LICENSE) - 查看 [LICENSE](LICENSE) 文件了解详情。

---

## 📞 联系我们

- **项目主页**: [GitHub Repository](https://github.com/your-username/smart-construction)
- **问题反馈**: [Issues](https://github.com/your-username/smart-construction/issues)
- **邮箱**: support@smartconstruction.com
- **技术支持**: 400-123-4567

---

## 🙏 致谢

感谢所有为这个项目做出贡献的开发者和用户！

- [.NET](https://dotnet.microsoft.com/) - 强大的开发框架
- [Vue.js](https://vuejs.org/) - 渐进式JavaScript框架
- [Element Plus](https://element-plus.org/) - Vue 3组件库
- [ECharts](https://echarts.apache.org/) - 数据可视化库

---

<div align="center">

**如果这个项目对你有帮助，请给它一个 ⭐️**

</div> 