using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("documents")]
public class Document
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }
    public MarriageApplication Application { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Category { get; set; } = DocumentCategories.Supporting;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
}
