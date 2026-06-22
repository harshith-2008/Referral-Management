using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs.Patient;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    // ================= DASHBOARD =================
    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        return new AdminDashboardDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalPatients = await _context.Patients.CountAsync(),
            TotalSpecialists = await _context.Specialists.CountAsync(),
            TotalReferrals = await _context.Referrals.CountAsync(),

            PendingReferrals = await _context.Referrals
                .CountAsync(r => r.ReferralStatus.StatusName == "Pending"),

            CompletedReferrals = await _context.Referrals
                .CountAsync(r => r.ReferralStatus.StatusName == "Completed"),

            CancelledReferrals = await _context.Referrals
                .CountAsync(r => r.ReferralStatus.StatusName == "Cancelled"),

            AppointmentsToday = await _context.Appointments
                .CountAsync(a => a.AppointmentDate == DateOnly.FromDateTime(DateTime.UtcNow))
        };
    }

    // ================= REFERRAL LEAKAGE =================
    public async Task<ReferralLeakageDto> GetReferralLeakageAsync()
    {
        var total = await _context.Referrals.CountAsync();

        var referralsWithoutAppointments = await _context.Referrals
            .Where(r => !r.Appointments.Any())
            .CountAsync();

        var delayed = await _context.Referrals
            .Where(r => r.Appointments.Any(a =>
r.CreatedAt.HasValue &&
a.AppointmentDate > DateOnly.FromDateTime(r.CreatedAt.Value.AddDays(5))
))
            .CountAsync();

        var notCompleted = await _context.Referrals
            .CountAsync(r => r.ReferralStatus.StatusName != "Completed");

        var leakage = referralsWithoutAppointments + delayed + notCompleted;

        return new ReferralLeakageDto
        {
            TotalReferrals = total,
            LeakageCount = leakage,
            LeakagePercentage = total == 0 ? 0 : (double)leakage / total * 100,
            NoAppointment = referralsWithoutAppointments,
            DelayedAppointments = delayed,
            NeverCompleted = notCompleted
        };
    }

    // ================= REFERRAL STATUS =================
    public async Task<List<StatusCountDto>> GetReferralStatusAsync()
    {
        return await _context.Referrals
            .GroupBy(r => r.ReferralStatus.StatusName)
            .Select(g => new StatusCountDto
            {
                Status = g.Key,
                Count = g.Count()
            }).ToListAsync();
    }

    // ================= FACILITY LEAKAGE =================
    public async Task<List<FacilityLeakageDto>> GetFacilityLeakageAsync()
    {
        return await _context.Referrals
            .GroupBy(r => r.OriginFacility.FacilityName)
            .Select(g => new FacilityLeakageDto
            {
                FacilityName = g.Key,
                TotalReferrals = g.Count(),
                LeakageCount = g.Count(r =>
                    !r.Appointments.Any() ||
                    r.ReferralStatus.StatusName != "Completed")
            }).ToListAsync();
    }

    // ================= SPECIALTY LOAD =================
    public async Task<List<SpecialtyLoadDto>> GetSpecialtyLoadAsync()
    {
        return await _context.Referrals
            .GroupBy(r => r.SpecialtyRequest.SpecialtyName)
            .Select(g => new SpecialtyLoadDto
            {
                Specialty = g.Key,
                ReferralCount = g.Count()
            }).ToListAsync();
    }

    // ================= APPOINTMENT ANALYTICS =================
    public async Task<AppointmentAnalyticsDto> GetAppointmentAnalyticsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return new AppointmentAnalyticsDto
        {
            TotalAppointments = await _context.Appointments.CountAsync(),
            Upcoming = await _context.Appointments.CountAsync(a => a.AppointmentDate >= today),
            Completed = await _context.Appointments
                .CountAsync(a => a.AppointmentStatus.StatusName == "Completed"),
            Missed = await _context.Appointments
                .CountAsync(a => a.AppointmentDate < today &&
                                 a.AppointmentStatus.StatusName != "Completed")
        };
    }

    // ================= DAILY REFERRALS =================
    public async Task<List<DailyReferralDto>> GetDailyReferralsAsync()
    {
        return await _context.Referrals
            .GroupBy(r => r.CreatedAt.Value.Date)
            .Select(g => new DailyReferralDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();
    }
}