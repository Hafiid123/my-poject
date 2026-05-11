using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("payments")]
public class Payment
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }
    public MarriageApplication Application { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int FeeId { get; set; }
    public Fee Fee { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required, MaxLength(30)]
    public string PaymentStatus { get; set; } = PaymentStatuses.Pending;

    public DateTime? PaymentDate { get; set; }

    [MaxLength(500)]
    public string? ReceiptImage { get; set; }

    [MaxLength(50)]
    public string? SenderPhone { get; set; }

    [MaxLength(100)]
    public string? TransactionNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
