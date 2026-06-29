using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;

/*
 * AppointmentController
 *
 * GET    /appointments/available-slots/{specialistId}/{date}    - Get available appointment slots for a specialist on a given date.
 * GET    /appointments/schedule/{date}                          - Get all appointments scheduled for the logged-in specialist on a given date.
 * POST   /appointments                                          - Create a new appointment for a referral.
 * GET    /appointments/{appointmentId}                          - Get detailed information about a specific appointment.
 * GET    /appointments/user                                     - Get all appointments for the logged-in user.
 * PUT    /appointments/update-status                            - Update the status of an appointment (Scheduled, Cancelled, Closed).
 */

namespace Referral_Management.Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "ReferralCoordinator")]
        [HttpGet("available-slots/{specialistId:int}/{date}")]
        public async Task<IActionResult> GetAvailableSlots(
            int specialistId,
            DateOnly date)
        {
            var coordinatorIdClaim =
                User.FindFirst("ReferralCoordinatorId")?.Value;

            if (string.IsNullOrEmpty(coordinatorIdClaim))
                return Unauthorized();

            int coordinatorId = int.Parse(coordinatorIdClaim);

            var result = await _appointmentService
                .GetAvailableSlotsAsync(specialistId, date, coordinatorId);

            return Ok(new ApiResponseDTO<AvailableSlotsResponseDTO>
            {
                Success = true,
                Message = "Available slots retrieved successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Specialist")]
        [HttpGet("schedule/{date}")]
        public async Task<IActionResult> GetSchedule(
            DateOnly date)
        {
            var specialistIdClaim = User.FindFirst("SpecialistId")?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            if (string.IsNullOrEmpty(specialistIdClaim))
                return Unauthorized();

            var specialistId = int.Parse(specialistIdClaim);

            var result = await _appointmentService
                .GetScheduleAsync(specialistId, date);

            return Ok(new ApiResponseDTO<DailyScheduleResponseDTO>
            {
                Success = true,
                Message = "Schedule retrieved successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "ReferralCoordinator")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] CreateAppointmentDTO request)
        {
            var coordinatorIdClaim =
                User.FindFirst("ReferralCoordinatorId")?.Value;

            if (string.IsNullOrEmpty(coordinatorIdClaim))
                return Unauthorized();

            int coordinatorId = int.Parse(coordinatorIdClaim);

            var result = await _appointmentService
                .CreateAppointmentAsync(request, coordinatorId);

            return Ok(new ApiResponseDTO<AppointmentResponseDTO>
            {
                Success = true,
                Message = "Appointment created successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Specialist")]
        [HttpGet("{appointmentId:int}")]
        public async Task<IActionResult> GetAppointmentDetails(
            int appointmentId)
        {
            var result = await _appointmentService
                .GetAppointmentDetailsAsync(appointmentId);

            return Ok(new ApiResponseDTO<AppointmentDetailsDTO>
            {
                Success = true,
                Message = "Appointment details retrieved successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserAppointments()
        {
            int userId = int.Parse(
    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
);
            var result = await _appointmentService
                .GetUserAppointmentsAsync(userId);

            return Ok(new ApiResponseDTO<List<UserAppointmentDTO>>
            {
                Success = true,
                Message = "Appointments retrieved successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Admin,ReferralCoordinator")]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateAppointmentStatus(
            [FromBody] UpdateAppointmentStatusDTO request)
        {
            var result = await _appointmentService
                .UpdateAppointmentStatusAsync(request);

            return Ok(new ApiResponseDTO<AppointmentStatusResponseDTO>
            {
                Success = true,
                Message = "Appointment status updated successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Specialist")]
        [HttpPut("{appointmentId:int}/complete")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            var specialistIdClaim = User.FindFirst("SpecialistId")?.Value;

            if (!int.TryParse(specialistIdClaim, out var specialistId))
                return Unauthorized();

            var result = await _appointmentService
                .MarkAppointmentCompletedAsync(appointmentId, specialistId);

            return Ok(new ApiResponseDTO<AppointmentStatusResponseDTO>
            {
                Success = true,
                Message = "Appointment marked as completed successfully.",
                Data = result
            });
        }
    }

}
