namespace online_mr_certi.Infrastructure;

/// <summary>Mobile money / manual payment instructions shown on checkout.</summary>
public class ManualPaymentOptions
{
    public const string SectionName = "ManualPayment";

    /// <summary>Number customers send payment to (e.g. 061XXXXXXX).</summary>
    public string MobileMoneyNumber { get; set; } = "0619979258";
}
