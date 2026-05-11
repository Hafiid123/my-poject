using System.ComponentModel.DataAnnotations.Schema;

namespace online_mr_certi.Models;

[Table("admin_notification_read_states")]
public class AdminNotificationReadState
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime LastReadAt { get; set; } = DateTime.MinValue;
}

