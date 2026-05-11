using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("marriage_witnesses")]
public class MarriageWitness
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }
    public MarriageApplication Application { get; set; } = null!;

    /// <summary>1 = first witness, 2 = second.</summary>
    public byte SortOrder { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Required, MaxLength(100)]
    public string IdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string ContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Address { get; set; } = string.Empty;
}
