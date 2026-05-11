using System.ComponentModel.DataAnnotations;
using online_mr_certi.Models;

namespace online_mr_certi.Models.ViewModels;

public class AdminUserEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = AppRoles.User;
}

public class AdminCreateUserViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public int RoleId { get; set; }
}

public class RolePermissionCheckboxViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public class RoleEditViewModel
{
    public int? Id { get; set; }

    [Required, MaxLength(100)]
    [Display(Name = "Role name")]
    public string Name { get; set; } = string.Empty;

    public List<RolePermissionCheckboxViewModel> Permissions { get; set; } = new();
}

public class AdminApplicationEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string HusbandName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime HusbandDob { get; set; }

    [Required, MaxLength(100)]
    public string HusbandIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string HusbandContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string HusbandAddress { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string WifeName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime WifeDob { get; set; }

    [Required, MaxLength(100)]
    public string WifeIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string WifeContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string WifeAddress { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime MarriageDate { get; set; }

    [Required, MaxLength(300)]
    public string MarriageLocation { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Status { get; set; } = ApplicationStatus.Pending;

    [MaxLength(2000)]
    public string? Remarks { get; set; }
}

public class FeeCreateViewModel
{
    [Required, MaxLength(200)]
    [Display(Name = "Service name")]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999.99)]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required, MaxLength(10)]
    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class FeeEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999.99)]
    public decimal Amount { get; set; }

    [Required, MaxLength(10)]
    [Display(Name = "Currency")]
    public string Currency { get; set; } = "USD";

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
