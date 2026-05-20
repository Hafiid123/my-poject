using online_mr_certi.Models;
namespace online_mr_certi.Services;
public interface ICertificatePdfService
{
    /// <summary>Writes a marriage certificate PDF to the given stream.</summary>
    void GenerateCertificatePdf(MarriageApplication application, Stream output, string webRootPath);
}
