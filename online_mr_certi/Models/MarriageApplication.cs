using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("marriage_applications")]
public class MarriageApplication
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required, MaxLength(200)]
    public string HusbandName { get; set; } = string.Empty;

    public DateTime HusbandDob { get; set; }

    [Required, MaxLength(100)]
    public string HusbandIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string HusbandContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string HusbandAddress { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string WifeName { get; set; } = string.Empty;

    public DateTime WifeDob { get; set; }

    [Required, MaxLength(100)]
    public string WifeIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string WifeContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string WifeAddress { get; set; } = string.Empty;

    public DateTime MarriageDate { get; set; }

    [Required, MaxLength(300)]
    public string MarriageLocation { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Status { get; set; } = ApplicationStatus.Pending;

    public DateTime SubmissionDate { get; set; }

    public DateTime? DecisionDate { get; set; }

    [MaxLength(2000)]
    public string? Remarks { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<MarriageWitness> Witnesses { get; set; } = new List<MarriageWitness>();
    public Payment? Payment { get; set; }
    public Certificate? Certificate { get; set; }
}
