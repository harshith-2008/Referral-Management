using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs.Patient;
using Referral_Management.Api.Exceptions;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using Referral_Management.Api.Services;
using Referral_Management.Api.DTOs.Admin;

namespace Referral_Management.Api.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OverviewAnalyticsDTO> GetOverviewAnalytics(
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                // ✅ Validate date range
                if (from.HasValue && to.HasValue && from > to)
                {
                    throw new BadRequestException("Invalid date range: 'from' cannot be greater than 'to'");
                }

                var referralsQuery = _context.Referrals.AsQueryable();

                // ✅ Apply filters
                if (from.HasValue)
                    referralsQuery = referralsQuery.Where(r => r.CreatedAt >= from);

                if (to.HasValue)
                    referralsQuery = referralsQuery.Where(r => r.CreatedAt <= to);

                // ✅ Load required data
                var referrals = await referralsQuery
                    .Include(r => r.ReferralStatus)
                    .ToListAsync();

                var totalReferrals = referrals.Count;

                var completed = referrals.Count(r =>
                    r.ReferralStatus.StatusName == "Completed" ||
                    r.ReferralStatus.StatusName == "Closed");

                var rejected = referrals.Count(r =>
                    r.ReferralStatus.StatusName == "Rejected");

                var pending = totalReferrals - completed - rejected;

                // ✅ Leakage calculation
                var leakagePercentage = totalReferrals == 0
                    ? 0
                    : (double)(totalReferrals - completed) / totalReferrals * 100;

                // ✅ Top specialties
                var topSpecialtiesRaw = referrals
                    .GroupBy(r => r.SpecialtyRequestId)
                    .Select(g => new
                    {
                        SpecialtyId = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                var specialties = await _context.Specialties.ToListAsync();

                var topSpecialties = topSpecialtiesRaw
                    .Select(ts => new SpecialtyAnalyticsDTO
                    {
                        SpecialtyId = ts.SpecialtyId,
                        SpecialtyName = specialties
                            .First(s => s.SpecialtyId == ts.SpecialtyId)
                            .SpecialtyName,
                        Count = ts.Count
                    })
                    .ToList();

                return new OverviewAnalyticsDTO
                {
                    TotalReferrals = totalReferrals,
                    TotalFacilities = await _context.Facilities.CountAsync(),
                    TotalPatients = await _context.Patients.CountAsync(),
                    TotalSpecialists = await _context.Specialists.CountAsync(),

                    CompletedReferrals = completed,
                    PendingReferrals = pending,
                    RejectedReferrals = rejected,

                    LeakagePercentage = Math.Round(leakagePercentage, 2),

                    TopSpecialties = topSpecialties
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching overview analytics", ex);
            }
        }
    }
}
