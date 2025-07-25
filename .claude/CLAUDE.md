# 项目级硬规则

- 禁止生成任何与业务无关的示例、mock 数据或测试页面
- 任何新文件必须先经 `src` 下对应层目录同意（Domain/Application/Infrastructure/Api）
- 所有 public 方法必须带 XML 注释，否则拒绝合并
- 添加任何文件都必须符合项目结构的整体规划，符合MASA framework编程规范，在项目对应的层级添加代码。任何代码在保存之前必须检查整个项目有没有同样功能的代码，如果功能重复必须禁止保存


# Cline + Kimi K2 专用规则（2025-07-25 生效）

# 文件路径：项目根目录/.claude/CLAUDE.md

# 作用范围：Cline 与 RooCode 均会读取

## 1 项目锁死

name: SmartConstruction
tech: .NET 8 + MASA Framework + Blazor + MQTT/TCP
goal: 智慧工地（塔吊、升降机、实名考勤、视频监控）

## 2 目录铁律

- 禁止在任何非指定目录创建文件
- 禁止重复实体/服务/DTO
- 禁止示例/mock/todo/fixme/Console.WriteLine/alert()

| 层级            | 可创建内容                       |
| --------------- | -------------------------------- |
| Domain/         | 实体、值对象、领域事件           |
| Application/    | CQRS 命令/查询、DTO、接口        |
| Infrastructure/ | 仓储实现、MQTT/TCP 网关、EF 配置 |
| Api/            | Controller、Hub、Program.cs      |
| Web/            | Blazor 页面、组件、服务          |
| tests/          | 单元/集成测试                    |

## 3 编码强制

- 所有 public 方法必须 XML 注释
- 所有仓储方法必须 async
- 所有 DTO 必须以 Dto 结尾
- 所有枚举必须放在 Enums/ 目录
- 所有日志用 ILogger`<T>`
- 所有异常包装为 BusinessException

## 4 上下文提示（每次对话自动注入）

You are Kimi-K2, **short-context** (128 K) agent.

- 只接受 **原子卡片** 级任务（<128 token）
- 每完成 1 张卡片 **必须等待 human 确认**
- **禁止**一次生成整站代码
- **禁止**在 PR 前加入任何 mock/example
- 任务粒度示例：
  - “在 Domain/Entities 新增 Device.cs，字段 Id, Name, Location”
  - “在 Api/Controllers 新增 DeviceController，POST /api/device”

## 5 工具限制

- Browser 工具：关闭（Disable browser tool usage）[^155^]
- 温度：0.6（官方推荐值）
- Max tokens：2048（防止过长）

## 6 记忆触发词

@plan   → 先出 3 步计划再写代码
@clean  → 删除未使用/示例代码
@guard  → 强制执行本规则检查

## 7 提交保护

- 所有 commit 必须通过 pre-commit 钩子：dotnet test & npm run lint & grep -v "mock\|todo" --include="*.cs" .
- PR 需 2 人 Review + CI 绿色
