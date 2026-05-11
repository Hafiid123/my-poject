using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("certificates")]
public class Certificate
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }
    public MarriageApplication Application { get; set; } = null!;

    [Required, MaxLength(500)]
    public string CertificateFile { get; set; } = string.Empty;
}
