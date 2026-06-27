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

    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize(Roles = "Specialist")]
        [HttpGet("lookup/{mrn}")]
        public async Task<IActionResult> GetPatientForReferral(string mrn)
        {
            var specialistIdClaim = User.FindFirst("SpecialistId")?.Value;

            if (!int.TryParse(specialistIdClaim, out var specialistId))
                return Unauthorized();

            var result = await _patientService
                .GetPatientForReferralAsync(mrn, specialistId);

            return Ok(new ApiResponseDTO<PatientReferralLookupDto>
            {
                Success = true,
                Message = "Patient found",
                Data = result
            });
        }

        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return int.Parse(userId);
        }

        // ===================== Dashboard =====================
        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Patient")]
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
