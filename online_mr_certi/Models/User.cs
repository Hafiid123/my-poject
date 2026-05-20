using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("users")]
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Role { get; set; } = AppRoles.User;

    public int? RoleId { get; set; }
    public Role? RoleEntity { get; set; }

    /// <summary>Set to Paid when a manual payment is approved by admin.</summary>
    [Required, MaxLength(30)]
    public string PaymentStatus { get; set; } = UserPaymentStatuses.Unpaid;

    public ICollection<MarriageApplication> MarriageApplications { get; set; } = new List<MarriageApplication>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();


}
