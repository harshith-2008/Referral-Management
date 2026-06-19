using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.DTOs.Patient;
using Referral_Management.Api.Exceptions;
using Referral_Management.Api.Services;
using System.Security.Claims;

namespace Referral_Management.Api.Controllers
{
    [ApiController]
    [Route("api/patient")]
    [Authorize(Roles = "Patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return int.Parse(userId);
        }

        // ===================== Dashboard =====================
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _patientService.GetDashboardAsync(GetUserId());

            return Ok(new ApiResponseDTO<PatientDashboardDto>
            {
                Success = true,
                Message = "Dashboard fetched successfully",
                Data = result
            });
        }

        // ===================== Profile =====================
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _patientService.GetProfileAsync(GetUserId());

            return Ok(new ApiResponseDTO<PatientProfileDto>
            {
                Success = true,
                Message = "Profile fetched successfully",
                Data = result
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDto dto)
        {
            var result = await _patientService.UpdateProfileAsync(GetUserId(), dto);

            return Ok(new ApiResponseDTO<string>
            {
                Success = true,
                Message = result,
                Data = null
            });
        }

        // ===================== Referrals =====================
        [HttpGet("referrals")]
        public async Task<IActionResult> GetReferrals()
        {
            var result = await _patientService.GetReferralsAsync(GetUserId());

            return Ok(new ApiResponseDTO<List<ReferralDto>>
            {
                Success = true,
                Message = "Referrals fetched successfully",
                Data = result
            });
        }

        [HttpGet("referrals/{id:int}")]
        public async Task<IActionResult> GetReferral(int id)
        {
            var result = await _patientService.GetReferralByIdAsync(GetUserId(), id);

            return Ok(new ApiResponseDTO<ReferralDetailsDto>
            {
                Success = true,
                Message = "Referral details fetched successfully",
                Data = result
            });
        }

        [HttpGet("referrals/{id:int}/status")]
        public async Task<IActionResult> GetReferralStatus(int id)
        {
            var result = await _patientService.GetReferralStatusAsync(GetUserId(), id);

            return Ok(new ApiResponseDTO<ReferralStatusDto>
            {
                Success = true,
                Message = "Referral status fetched successfully",
                Data = result
            });
        }

        // ===================== Appointments =====================
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var result = await _patientService.GetAppointmentsAsync(GetUserId());

            return Ok(new ApiResponseDTO<List<AppointmentDto>>
            {
                Success = true,
                Message = "Appointments fetched successfully",
                Data = result
            });
        }

        [HttpGet("appointments/upcoming")]
        public async Task<IActionResult> GetUpcomingAppointments()
        {
            var result = await _patientService.GetUpcomingAppointmentsAsync(GetUserId());

            return Ok(new ApiResponseDTO<List<AppointmentDto>>
            {
                Success = true,
                Message = "Upcoming appointments fetched successfully",
                Data = result
            });
        }

        [HttpGet("appointments/history")]
        public async Task<IActionResult> GetAppointmentHistory()
        {
            var result = await _patientService.GetAppointmentHistoryAsync(GetUserId());

            return Ok(new ApiResponseDTO<List<AppointmentDto>>
            {
                Success = true,
                Message = "Appointment history fetched successfully",
                Data = result
            });
        }

        [HttpGet("appointments/{id:int}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var result = await _patientService.GetAppointmentByIdAsync(GetUserId(), id);

            return Ok(new ApiResponseDTO<AppointmentDetailsDto>
            {
                Success = true,
                Message = "Appointment details fetched successfully",
                Data = result
            });
        }
    }
}