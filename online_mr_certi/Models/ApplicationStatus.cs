namespace online_mr_certi.Models;

public static class ApplicationStatus
{
    /// <summary>Awaiting fee payment and receipt submission.</summary>
    public const string PendingPayment = "Pending Payment";

    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
}
