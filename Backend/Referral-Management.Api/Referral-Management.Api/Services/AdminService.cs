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
    public async Task<List<UserListDto>> GetUsersAsync()
    {
        return await _context.Users
            .Where(u => u.Role.RoleName != "Admin")
            .Select(u => new UserListDto
            {
                UserId = u.UserId,
                Name = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Role = u.Role.RoleName,
                Facility = u.Facility.FacilityName
            })
            .ToListAsync();
    }

    // ================= DASHBOARD =================

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var nowTime = TimeOnly.FromDateTime(DateTime.Now);
        var totalUsers = await _context.Users.CountAsync(u => u.Role.RoleName != "Admin");
        var totalPatients = await _context.Patients.CountAsync();
        var totalSpecialists = await _context.Specialists.CountAsync();
        var totalReferrals = await _context.Referrals.CountAsync();
        var totalFacilities = await _context.Facilities.CountAsync();

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalPatients = totalPatients,
            TotalSpecialists = totalSpecialists,
            TotalReferrals = totalReferrals,

            PendingReferrals = await _context.Referrals.CountAsync(r =>
                r.ReferralStatus.StatusName == "Requested" ||
                r.ReferralStatus.StatusName == "Submitted" ||
                r.ReferralStatus.StatusName == "Accepted"
),


            CompletedReferrals = await _context.Referrals
                .CountAsync(r => r.ReferralStatus.StatusName == "Completed"),

            CancelledReferrals = await _context.Referrals
                .CountAsync(r => r.ReferralStatus.StatusName == "Rejected" ||
                                 r.ReferralStatus.StatusName == "Closed"),

            AppointmentsToday = await _context.Appointments
                .CountAsync(a => a.AppointmentDate == today && a.AppointmentTime > nowTime),

            ReferralsPerPatient = totalPatients == 0 ? 0 : (double)totalReferrals / totalPatients,
            AverageReferralsPerFacility = totalFacilities == 0 ? 0 : (double)totalReferrals / totalFacilities
        };
    }


    // ================= REFERRAL LEAKAGE =================
    public async Task<ReferralLeakageDto> GetReferralLeakageAsync()
    {
        // ✅ Total referrals from your hospital
        var total = await _context.Referrals
            .Where(r => r.OriginFacility.HospitalId == 1)
            .CountAsync();

        // ✅ Out-of-network routing
        var outOfNetwork = await _context.Referrals
            .Where(r =>
                r.OriginFacility.HospitalId == 1 &&
                r.DestinationFacility.HospitalId != 1
            )
            .Select(r => r.ReferralId)
            .ToListAsync();

        // ✅ Rejected referrals (treated as leakage ✅)
        var rejected = await _context.Referrals
            .Where(r =>
                r.OriginFacility.HospitalId == 1 &&
                r.ReferralStatus.StatusName == "Rejected"
            )
            .Select(r => r.ReferralId)
            .ToListAsync();

        // ✅ Combine WITHOUT duplicates
        var leakageIds = outOfNetwork
            .Union(rejected)
            .Distinct()
            .ToList();

        var leakageCount = leakageIds.Count;

        return new ReferralLeakageDto
        {
            TotalReferrals = total,
            LeakageCount = leakageCount,
            LeakagePercentage = total == 0 ? 0 : (double)leakageCount / total * 100,

            // ✅ Breakdown
            OutOfNetwork = outOfNetwork.Count,
            NeverCompleted = rejected.Count, // ✅ reuse field for rejected

            // ✅ Not used anymore
            NoAppointment = 0,
            DelayedAppointments = 0
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
            .Where(r => r.OriginFacility.HospitalId == 1) // ✅ only your hospital
            .GroupBy(r => r.OriginFacility.FacilityName)
            .Select(g => new FacilityLeakageDto
            {
                FacilityName = g.Key,
                TotalReferrals = g.Count(),

                // ✅ Leakage = Out-of-network OR Rejected
                LeakageCount = g.Count(r =>
                    r.DestinationFacility.HospitalId != 1 ||
                    r.ReferralStatus.StatusName == "Rejected"
                )
            })
            .ToListAsync();
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
        var today = DateOnly.FromDateTime(DateTime.Now);
        var nowTime = TimeOnly.FromDateTime(DateTime.Now);

        return new AppointmentAnalyticsDto
        {
            TotalAppointments = await _context.Appointments.CountAsync(),
            Upcoming = await _context.Appointments.CountAsync(a =>
                a.AppointmentDate > today ||
                (a.AppointmentDate == today && a.AppointmentTime > nowTime)),
            Completed = await _context.Appointments
                .CountAsync(a => a.AppointmentStatus.StatusName == "Completed"),
            Missed = await _context.Appointments
                .CountAsync(a =>
                    (a.AppointmentDate < today ||
                     (a.AppointmentDate == today && a.AppointmentTime <= nowTime)) &&
                    a.AppointmentStatus.StatusName != "Completed")
        };
    }

    // ================= DAILY REFERRALS =================

    public async Task<List<DailyReferralDto>> GetDailyReferralsAsync()
    {
        return await _context.Referrals
            .Where(r => r.CreatedAt.HasValue) // ✅ safety
            .GroupBy(r => r.CreatedAt.Value.Date)
            .Select(g => new DailyReferralDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Date) // ✅ latest first
            .Take(5)                        // ✅ last 5 days ONLY
            .OrderBy(x => x.Date)           // ✅ reorder ascending for UI
            .ToListAsync();
    }
    public async Task<List<MonthlyReferralDto>> GetMonthlyReferralAsync()
    {
        return await _context.Referrals
            .Where(r => r.CreatedAt.HasValue)
            .GroupBy(r => new
            {
                r.CreatedAt.Value.Year,
                r.CreatedAt.Value.Month
            })
            .Select(g => new MonthlyReferralDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();
    }
    public async Task<List<TopSpecialistDto>> GetTopSpecialistsAsync()
    {
        return await _context.Referrals
            .GroupBy(r => r.FromSpecialist.User.FirstName + " " + r.FromSpecialist.User.LastName)
            .Select(g => new TopSpecialistDto
            {
                Specialist = g.Key,
                Referrals = g.Count()
            })
            .OrderByDescending(x => x.Referrals)
            .Take(5)
            .ToListAsync();
    }
    public async Task<ReferralAgingDto> GetReferralAgingAsync()
    {
        var activeReferrals = await _context.Referrals
            .Where(r =>
                r.CreatedAt.HasValue &&
                (
                    r.ReferralStatus.StatusName == "Requested" ||
                    r.ReferralStatus.StatusName == "Submitted" ||
                    r.ReferralStatus.StatusName == "Accepted"
                )
            )
            .Select(r => new
            {
                days = (DateTime.UtcNow - r.CreatedAt.Value).Days
            })
            .ToListAsync();

        return new ReferralAgingDto
        {
            LessThan3 = activeReferrals.Count(x => x.days <= 3),
            Between3And7 = activeReferrals.Count(x => x.days > 3 && x.days <= 7),
            MoreThan7 = activeReferrals.Count(x => x.days > 7)
        };
    }
    public async Task<ScheduledDelayDto> GetScheduledDelaysAsync()
    {
        var scheduled = await _context.Referrals
            .Where(r =>
                r.CreatedAt.HasValue &&
                r.ReferralStatus.StatusName == "Accepted"
            )
            .Select(r => new
            {
                days = (DateTime.UtcNow - r.CreatedAt.Value).Days
            })
            .ToListAsync();

        return new ScheduledDelayDto
        {
            TotalScheduled = scheduled.Count,
            Delayed = scheduled.Count(x => x.days > 7),
            Healthy = scheduled.Count(x => x.days <= 7)
        };
    }

}
