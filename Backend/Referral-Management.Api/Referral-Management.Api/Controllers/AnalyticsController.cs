using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;
using Referral_Management.Api.DTOs.Admin;

namespace Referral_Management.Api.Controllers
{
    [ApiController]
    [Route("api/admin/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _service;

        public AnalyticsController(IAnalyticsService service)
        {
            _service = service;
        }

        /// <summary>
        /// ✅ Overall system analytics across all facilities
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverviewAnalytics(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var data = await _service.GetOverviewAnalytics(from, to);

            var response = new ApiResponseDTO<OverviewAnalyticsDTO>
            {
                Success = true,
                Message = "Overview analytics fetched successfully",
                Data = data
            };

            return Ok(response);
        }
    }
}
