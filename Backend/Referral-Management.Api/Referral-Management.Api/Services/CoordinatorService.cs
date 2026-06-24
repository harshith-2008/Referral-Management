using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Services;

public class CoordinatorService : ICoordinatorService
{
    private readonly AppDbContext _context;

    public CoordinatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CoordinatorDashboardDto>
        GetDashboardAsync(int coordinatorId)
    {
        var referrals = await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.ReferralStatus)
            .Where(r => r.CreatedByCoordinatorId == coordinatorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return new CoordinatorDashboardDto
        {
            TotalReferrals = referrals.Count,

            Submitted = referrals.Count(r =>
                r.ReferralStatus.StatusName == "Submitted"),

            Requested = referrals.Count(r =>
                r.ReferralStatus.StatusName == "Requested"),

            Accepted = referrals.Count(r =>
                r.ReferralStatus.StatusName == "Accepted"),

            Rejected = referrals.Count(r =>
                r.ReferralStatus.StatusName == "Rejected"),

            Closed = referrals.Count(r =>
                r.ReferralStatus.StatusName == "Closed"),

            RecentReferrals = referrals
                .Take(5)
                .Select(r => new ReferralDto
                {
                    ReferralId = r.ReferralId,

                    PatientName =
                        $"{r.Patient.User.FirstName} {r.Patient.User.LastName}",

                    Status = r.ReferralStatus.StatusName,

                    CreatedAt = r.CreatedAt!.Value.Date
                })
                .ToList()
        };
    }
}