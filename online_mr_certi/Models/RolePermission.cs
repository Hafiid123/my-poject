using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("role_permissions")]
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

