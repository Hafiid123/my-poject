namespace online_mr_certi.Models.ViewModels;

public class AdminNotificationsViewModel
{
    public int UnreadCount { get; set; }
    public List<AdminNotificationItemViewModel> Alerts { get; set; } = new();
    public List<AdminNotificationItemViewModel> Notifications { get; set; } = new();
}

public class AdminNotificationItemViewModel
{
    public string IconClass { get; set; } = "fas fa-info-circle";
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; } = "/Admin/Index";
    public bool IsAlert { get; set; }
}

