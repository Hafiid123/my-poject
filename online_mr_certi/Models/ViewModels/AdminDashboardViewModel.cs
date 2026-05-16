namespace online_mr_certi.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalApplications { get; set; }
    public int PendingPayment { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }

    public List<string> TrendLabels { get; set; } = new();
    public List<int> TrendValues { get; set; } = new();

    public int StatusApproved { get; set; }
    public int StatusRejected { get; set; }
    public int StatusPending { get; set; }

    public double AverageProcessingHours { get; set; }

    public int OverdueApplicationsCount { get; set; }
    public string OverdueStatusFilter { get; set; } = ApplicationStatus.Pending;

    public string? SearchQuery { get; set; }
    public string? StatusFilter { get; set; }
    public List<AdminDashboardApplicationRow> FilteredApplications { get; set; } = new();

    public List<ActivityItemViewModel> RecentActivities { get; set; } = new();


    // ===== Payment Chart =====
    public List<string> PaymentTrendLabels { get; set; } = new();
    public List<int> PaymentTrendValues { get; set; } = new();
    public int TotalPaidPayments { get; set; }
    public int TotalPendingPayments { get; set; }
    public decimal TotalRevenue { get; set; }
}



public class AdminDashboardApplicationRow
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
}

public class ActivityItemViewModel
{
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

