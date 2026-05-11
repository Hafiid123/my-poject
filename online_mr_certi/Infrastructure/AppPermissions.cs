namespace online_mr_certi.Infrastructure;

public static class AppPermissions
{
    public const string ViewDashboard = "ViewDashboard";
    public const string CreateApplication = "CreateApplication";
    public const string ViewApplication = "ViewApplication";
    public const string ApproveApplications = "ApproveApplications";
    public const string RejectApplications = "RejectApplications";
    public const string IssueCertificate = "IssueCertificate";
    public const string ManageUsers = "ManageUsers";
    public const string ManageRoles = "ManageRoles";
    public const string ManageFees = "ManageFees";
    public const string ManagePayments = "ManagePayments";

    public static readonly string[] All =
    [
        ViewDashboard,
        CreateApplication,
        ViewApplication,
        ApproveApplications,
        RejectApplications,
        IssueCertificate,
        ManageUsers,
        ManageRoles,
        ManageFees,
        ManagePayments
    ];
}

