using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public int FacilityId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual Facility Facility { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<ReferralCoordinator> ReferralCoordinators { get; set; } = new List<ReferralCoordinator>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Specialist> Specialists { get; set; } = new List<Specialist>();
}
