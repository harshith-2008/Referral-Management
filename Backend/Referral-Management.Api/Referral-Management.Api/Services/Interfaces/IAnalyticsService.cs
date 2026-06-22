using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Referral_Management.Api.DTOs.Admin;

namespace Referral_Management.Api.Services.Interfaces
{
    public interface IAnalyticsService
    {

        Task<OverviewAnalyticsDTO> GetOverviewAnalytics(
                    DateTime? from = null,
                    DateTime? to = null
                );


    }
}

