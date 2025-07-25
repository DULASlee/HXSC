namespace SmartConstruction.Contracts.Dtos.User;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class AssignRolesRequest
{
    [Required(ErrorMessage = "角色ID列表不能为空")]
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}