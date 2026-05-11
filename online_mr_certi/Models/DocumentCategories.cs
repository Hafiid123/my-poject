namespace online_mr_certi.Models;

/// <summary>Stored on <see cref="Document"/> to classify uploads.</summary>
public static class DocumentCategories
{
    public const string Supporting = "Supporting";

    public const string HusbandIdentityDocument = "HusbandIdentityDocument";
    public const string WifeIdentityDocument = "WifeIdentityDocument";
    public const string Witness1IdentityDocument = "Witness1IdentityDocument";
    public const string Witness2IdentityDocument = "Witness2IdentityDocument";

    public const string HusbandPassportPhoto = "HusbandPassportPhoto";
    public const string WifePassportPhoto = "WifePassportPhoto";

    /// <summary>Stable ordering when listing documents on screen.</summary>
    public static int SortOrder(string category) => category switch
    {
        HusbandIdentityDocument => 1,
        WifeIdentityDocument => 2,
        Witness1IdentityDocument => 3,
        Witness2IdentityDocument => 4,
        HusbandPassportPhoto => 5,
        WifePassportPhoto => 6,
        Supporting => 90,
        _ => 80
    };

    public static string DisplayName(string category) => category switch
    {
        HusbandIdentityDocument => "Husband ID / Passport",
        WifeIdentityDocument => "Wife ID / Passport",
        Witness1IdentityDocument => "Witness 1 ID",
        Witness2IdentityDocument => "Witness 2 ID",
        HusbandPassportPhoto => "Husband passport-size photo",
        WifePassportPhoto => "Wife passport-size photo",
        Supporting => "Supporting document",
        _ => category
    };
}
