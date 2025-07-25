using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities;

/// <summary>
/// 租户实体
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>
    /// 租户编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 租户名称
    /// </summary>
    public string Name { get; set; } = string.Empty;



    /// <summary>
    /// 状态
    /// </summary>
    public byte Status { get; set; } = 1;

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 隔离模式
    /// </summary>
    public string IsolationMode { get; set; } = "DATABASE";

    /// <summary>
    /// Logo
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    public string? Theme { get; set; }

    // 其他字段可根据需要扩展
}
