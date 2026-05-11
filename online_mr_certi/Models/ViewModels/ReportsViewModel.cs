namespace online_mr_certi.Models.ViewModels;

public class ReportsViewModel
{
    public List<MonthlyReportRow> ByMonth { get; set; } = new();
    public List<MarriageApplicationSummary> RecentApplications { get; set; } = new();
}

public class MonthlyReportRow
{
    public string Period { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}

public class MarriageApplicationSummary
{
    public int Id { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty;
    public string HusbandName { get; set; } = string.Empty;
    public string WifeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
}
