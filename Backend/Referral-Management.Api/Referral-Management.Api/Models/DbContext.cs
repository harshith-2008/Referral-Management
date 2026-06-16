using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Referral_Management.Api.Models;

public partial class DbContext : DbContext
{
    public DbContext()
    {
    }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<GlobalNetwork> GlobalNetworks { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Referral> Referrals { get; set; }

    public virtual DbSet<ReferralAssignment> ReferralAssignments { get; set; }

    public virtual DbSet<ReferralCoordinator> ReferralCoordinators { get; set; }

    public virtual DbSet<ReferralStatus> ReferralStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShiftBlock> ShiftBlocks { get; set; }

    public virtual DbSet<Specialist> Specialists { get; set; }

    public virtual DbSet<SpecialistSpeciality> SpecialistSpecialities { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<UrgencyLevel> UrgencyLevels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Referral-Management;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE488AE72CCCF");

            entity.ToTable("Admin");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_User");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC21BD15C85");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentTime).HasPrecision(0);

            entity.HasOne(d => d.AppointmentStatus).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AppointmentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_AppointmentStatus");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Patient");

            entity.HasOne(d => d.Referral).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ReferralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Referral");

            entity.HasOne(d => d.Specialist).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SpecialistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Specialist");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.AppointmentStatusId).HasName("PK__Appointm__A619B660DA285B71");

            entity.ToTable("AppointmentStatus");

            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDF60A6BA6");

            entity.ToTable("AuditLog");

            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ActionUserNavigation).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ActionUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");

            entity.HasOne(d => d.Referral).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ReferralId)
                .HasConstraintName("FK_AuditLog_Referral");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__Facility__5FB08A7487A31EA6");

            entity.ToTable("Facility");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FacilityName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Hospital).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_Hospital");
        });

        modelBuilder.Entity<GlobalNetwork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GlobalNe__3214EC076383A91C");

            entity.ToTable("GlobalNetwork");

            entity.Property(e => e.NetworkName)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.HospitalId).HasName("PK__Hospital__38C2E5AF45D49CEC");

            entity.ToTable("Hospital");

            entity.Property(e => e.HeadOfficeAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HospitalName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

            entity.HasOne(d => d.GlobalNetwork).WithMany(p => p.Hospitals)
                .HasForeignKey(d => d.GlobalNetworkId)
                .HasConstraintName("FK_Hospital_GlobalNetwork");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC3664CF8367F");

            entity.ToTable("Patient");

            entity.HasIndex(e => e.Mrn, "UQ__Patient__C797E1BCD109ABB3").IsUnique();

            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InsuranceProviderName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.InsuranceStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mrn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PatientAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.PrimaryFacility).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PrimaryFacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_Facility");

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_User");
        });

        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.ReferralId).HasName("PK__Referral__A2C4A966CB113039");

            entity.ToTable("Referral");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DiagnosisCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ReferralReason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByCoordinator).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.CreatedByCoordinatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_Coordinator");

            entity.HasOne(d => d.DestinationFacility).WithMany(p => p.ReferralDestinationFacilities)
                .HasForeignKey(d => d.DestinationFacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_DestinationFacility");

            entity.HasOne(d => d.FromSpecialist).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.FromSpecialistId)
                .HasConstraintName("FK_Referral_FromSpecialist");

            entity.HasOne(d => d.OriginFacility).WithMany(p => p.ReferralOriginFacilities)
                .HasForeignKey(d => d.OriginFacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_OriginFacility");

            entity.HasOne(d => d.Patient).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_Patient");

            entity.HasOne(d => d.ReferralStatus).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.ReferralStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_ReferralStatus");

            entity.HasOne(d => d.SpecialtyRequest).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.SpecialtyRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_Specialty");

            entity.HasOne(d => d.UrgencyLevel).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.UrgencyLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_UrgencyLevel");
        });

        modelBuilder.Entity<ReferralAssignment>(entity =>
        {
            entity.HasKey(e => e.ReferralAssignmentId).HasName("PK__Referral__CB0E6EB44D0593D0");

            entity.ToTable("ReferralAssignment");

            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ReassignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.ReferralAssignments)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralAssignment_AssignedBy");

            entity.HasOne(d => d.FromSpecialist).WithMany(p => p.ReferralAssignmentFromSpecialists)
                .HasForeignKey(d => d.FromSpecialistId)
                .HasConstraintName("FK_ReferralAssignment_FromSpecialist");

            entity.HasOne(d => d.Referral).WithMany(p => p.ReferralAssignments)
                .HasForeignKey(d => d.ReferralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralAssignment_Referral");

            entity.HasOne(d => d.ToSpecialist).WithMany(p => p.ReferralAssignmentToSpecialists)
                .HasForeignKey(d => d.ToSpecialistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralAssignment_ToSpecialist");
        });

        modelBuilder.Entity<ReferralCoordinator>(entity =>
        {
            entity.HasKey(e => e.ReferralCoordinatorId).HasName("PK__Referral__E8AC3BF8EDCA20A2");

            entity.ToTable("ReferralCoordinator");

            entity.HasOne(d => d.Facility).WithMany(p => p.ReferralCoordinators)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralCoordinator_Facility");

            entity.HasOne(d => d.User).WithMany(p => p.ReferralCoordinators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralCoordinator_User");
        });

        modelBuilder.Entity<ReferralStatus>(entity =>
        {
            entity.HasKey(e => e.ReferralStatusId).HasName("PK__Referral__2B43ECB55C1119B7");

            entity.ToTable("ReferralStatus");

            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A57881FCE");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShiftBlock>(entity =>
        {
            entity.HasKey(e => e.ShiftBlockId).HasName("PK__ShiftBlo__82DDFDA0970F299A");

            entity.ToTable("ShiftBlock");

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.StartTime).HasPrecision(0);
        });

        modelBuilder.Entity<Specialist>(entity =>
        {
            entity.HasKey(e => e.SpecialistId).HasName("PK__Speciali__7092086EB5E7A854");

            entity.ToTable("Specialist");

            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Facility).WithMany(p => p.Specialists)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specialist_Facility");

            entity.HasOne(d => d.ShiftBlock).WithMany(p => p.Specialists)
                .HasForeignKey(d => d.ShiftBlockId)
                .HasConstraintName("FK_Specialist_ShiftBlock");

            entity.HasOne(d => d.User).WithMany(p => p.Specialists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specialist_User");
        });

        modelBuilder.Entity<SpecialistSpeciality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Speciali__3214EC07ED110C0F");

            entity.HasIndex(e => new { e.SpecialistId, e.SpecialtyId }, "UQ_SpecialistSpecialty").IsUnique();

            entity.HasOne(d => d.Specialist).WithMany(p => p.SpecialistSpecialities)
                .HasForeignKey(d => d.SpecialistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpecialistSpecialities_Specialist");

            entity.HasOne(d => d.Specialty).WithMany(p => p.SpecialistSpecialities)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpecialistSpecialities_Specialty");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6A8B1716332");

            entity.ToTable("Specialty");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SpecialtyName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UrgencyLevel>(entity =>
        {
            entity.HasKey(e => e.UrgencyLevelId).HasName("PK__UrgencyL__0CA733F95D6E77B7");

            entity.ToTable("UrgencyLevel");

            entity.Property(e => e.LevelName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CE8D4A6F6");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534353BA289").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Facility).WithMany(p => p.Users)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Facility");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
