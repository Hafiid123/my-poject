using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace online_mr_certi.Models.ViewModels;

public class WitnessFormModel
{
    [Required, MaxLength(200)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime DateOfBirth { get; set; }

    [Required, MaxLength(100)]
    [Display(Name = "National ID")]
    public string IdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    [Display(Name = "Contact number")]
    public string ContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;
}

public class MarriageApplicationCreateViewModel : IValidatableObject
{
    [Required, MaxLength(200)]
    [Display(Name = "Husband full name")]
    public string HusbandName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Husband date of birth")]
    public DateTime HusbandDob { get; set; }

    [Required, MaxLength(100)]
    [Display(Name = "Husband National ID ")]
    public string HusbandIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    [Display(Name = "Husband contact number")]
    public string HusbandContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    [Display(Name = "Husband address")]
    public string HusbandAddress { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    [Display(Name = "Wife full name")]
    public string WifeName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Wife date of birth")]
    public DateTime WifeDob { get; set; }

    [Required, MaxLength(100)]
    [Display(Name = "Wife National ID")]
    public string WifeIdNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    [Display(Name = "Wife contact number")]
    public string WifeContactNumber { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    [Display(Name = "Wife address")]
    public string WifeAddress { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Marriage date")]
    public DateTime MarriageDate { get; set; }

    [Required, MaxLength(300)]
    [Display(Name = "Marriage location")]
    public string MarriageLocation { get; set; } = string.Empty;

    [Display(Name = "Witness 1 (Markhaatiye 1)")]
    public WitnessFormModel Witness1 { get; set; } = new();

    [Display(Name = "Witness 2 (Markhaatiye 2)")]
    public WitnessFormModel Witness2 { get; set; } = new();

    [Display(Name = "Husband ID / Passport (scan or photo)")]
    public IFormFile? HusbandIdentityDocument { get; set; }

    [Display(Name = "Wife ID / Passport (scan or photo)")]
    public IFormFile? WifeIdentityDocument { get; set; }

    [Display(Name = "Witness 1 ID (scan or photo)")]
    public IFormFile? Witness1IdentityDocument { get; set; }

    [Display(Name = "Witness 2 ID (scan or photo)")]
    public IFormFile? Witness2IdentityDocument { get; set; }

    [Display(Name = "Husband passport-size photo")]
    public IFormFile? HusbandPassportPhoto { get; set; }

    [Display(Name = "Wife passport-size photo")]
    public IFormFile? WifePassportPhoto { get; set; }

    [Display(Name = "Additional supporting documents (PDF, JPG, PNG — optional)")]
    public List<IFormFile>? UploadedFiles { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var err in ValidateWitness(Witness1, nameof(Witness1), validationContext))
            yield return err;
        foreach (var err in ValidateWitness(Witness2, nameof(Witness2), validationContext))
            yield return err;

        foreach (var err in ValidateRequiredUpload(HusbandIdentityDocument, nameof(HusbandIdentityDocument)))
            yield return err;
        foreach (var err in ValidateRequiredUpload(WifeIdentityDocument, nameof(WifeIdentityDocument)))
            yield return err;
        foreach (var err in ValidateRequiredUpload(Witness1IdentityDocument, nameof(Witness1IdentityDocument)))
            yield return err;
        foreach (var err in ValidateRequiredUpload(Witness2IdentityDocument, nameof(Witness2IdentityDocument)))
            yield return err;
        foreach (var err in ValidateRequiredUpload(HusbandPassportPhoto, nameof(HusbandPassportPhoto)))
            yield return err;
        foreach (var err in ValidateRequiredUpload(WifePassportPhoto, nameof(WifePassportPhoto)))
            yield return err;
    }

    private static IEnumerable<ValidationResult> ValidateRequiredUpload(IFormFile? file, string memberName)
    {
        if (file is null || file.Length == 0)
            yield return new ValidationResult("This file is required.", new[] { memberName });
    }

    private static IEnumerable<ValidationResult> ValidateWitness(
        WitnessFormModel witness,
        string prefix,
        ValidationContext parent)
    {
        var ctx = new ValidationContext(witness, parent, parent.Items);
        var errs = new List<ValidationResult>();
        if (Validator.TryValidateObject(witness, ctx, errs, validateAllProperties: true))
            yield break;

        foreach (var e in errs)
        {
            foreach (var member in e.MemberNames.DefaultIfEmpty(string.Empty))
            {
                var key = string.IsNullOrEmpty(member) ? prefix : $"{prefix}.{member}";
                yield return new ValidationResult(e.ErrorMessage ?? "Invalid value.", new[] { key });
            }
        }
    }
}

public class AdminRejectViewModel
{
    [Required]
    public int Id { get; set; }

    [MaxLength(2000)]
    public string? Remarks { get; set; }
}

public class PaymentCheckoutViewModel
{
    public int ApplicationId { get; set; }

    [Display(Name = "Fee")]
    public string FeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    /// <summary>Configured payment number (display only).</summary>
    public string MobileMoneyNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    [Display(Name = "Sender phone number")]
    public string SenderPhone { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    [Display(Name = "Transaction number")]
    public string TransactionNumber { get; set; } = string.Empty;

    [Display(Name = "Receipt image (JPG or PNG)")]
    public IFormFile? ReceiptImage { get; set; }
}
