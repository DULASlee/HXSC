namespace SmartConstruction.Contracts.Dtos.User;

using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; } = null!;
}