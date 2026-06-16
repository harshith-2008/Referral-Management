using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class AuditLog
{
    public int AuditLogId { get; set; }

    public int ActionUser { get; set; }

    public int? ReferralId { get; set; }

    public string EntityType { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User ActionUserNavigation { get; set; } = null!;

    public virtual Referral? Referral { get; set; }
}
