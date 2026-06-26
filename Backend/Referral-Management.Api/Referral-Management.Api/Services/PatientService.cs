using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs.Patient;
using Referral_Management.Api.Exceptions;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientReferralLookupDto> GetPatientForReferralAsync(string mrn)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.PrimaryFacility)
                .FirstOrDefaultAsync(p => p.Mrn == mrn);

            if (patient == null)
                throw new NotFoundException("Patient not found.");

            return new PatientReferralLookupDto
            {
                PatientId = patient.PatientId,

                Mrn = patient.Mrn,

                FullName =
                    patient.User.FirstName + " " +
                    patient.User.LastName,

                Dob = patient.Dob,

                Gender = patient.Gender,

                FacilityName =
                    patient.PrimaryFacility?.FacilityName ?? string.Empty
            };
        }

        //==========================================================
        // Helper
        //==========================================================
        private async Task<Patient> GetPatientEntityAsync(int userId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.PrimaryFacility)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                throw new NotFoundException("Patient not found.");

            return patient;
        }

        //==========================================================
        // PROFILE
        //==========================================================
        public async Task<PatientProfileDto> GetProfileAsync(int userId)
        {
            var p = await GetPatientEntityAsync(userId);

            return new PatientProfileDto
            {
                PatientId = p.PatientId,
                Mrn = p.Mrn,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                PhoneNumber = p.User.PhoneNumber,
                Dob = p.Dob,
                Gender = p.Gender,
                InsuranceProviderName = p.InsuranceProviderName ?? "",
                InsuranceStatus = p.InsuranceStatus ?? "",
                PatientAddress = p.PatientAddress,
                FacilityName = p.PrimaryFacility?.FacilityName ?? "",
                Status = p.Status
            };
        }

        public async Task<string> UpdateProfileAsync(int userId, UpdatePatientProfileDto dto)
        {
            var p = await GetPatientEntityAsync(userId);

            p.User.PhoneNumber = dto.PhoneNumber;
            p.PatientAddress = dto.PatientAddress;
            p.InsuranceProviderName = dto.InsuranceProviderName;
            p.InsuranceStatus = dto.InsuranceStatus;

            p.User.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return "Profile updated successfully.";
        }

        //==========================================================
        // DASHBOARD
        //==========================================================
        public async Task<PatientDashboardDto> GetDashboardAsync(int userId)
        {
            var patient = await GetPatientEntityAsync(userId);

            var referrals = await _context.Referrals
                .Where(r => r.PatientId == patient.PatientId)
                .Include(r => r.ReferralStatus)
                .Include(r => r.SpecialtyRequest)
                .Include(r => r.UrgencyLevel)
                .Include(r => r.OriginFacility)
                .Include(r => r.DestinationFacility)
                .ToListAsync();

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patient.PatientId)
                .Include(a => a.Specialist).ThenInclude(s => s.User)
                .Include(a => a.Specialist).ThenInclude(s => s.Facility)
                .Include(a => a.AppointmentStatus)
                .Include(a => a.Referral).ThenInclude(r => r.SpecialtyRequest)
                .ToListAsync();

            var upcoming = appointments
                .Where(a => a.AppointmentDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                .ToList();

            return new PatientDashboardDto
            {
                Profile = await GetProfileAsync(userId),

                TotalReferrals = referrals.Count,

                PendingReferrals = referrals.Count(r => r.ReferralStatus.StatusName == "Pending"),

                CompletedReferrals = referrals.Count(r => r.ReferralStatus.StatusName == "Completed"),

                UpcomingAppointments = upcoming.Count,

                UpcomingAppointmentList = upcoming.Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                    FacilityName = a.Specialist.Facility.FacilityName,
                    Specialty = a.Referral.SpecialtyRequest.SpecialtyName,
                    AppointmentStatus = a.AppointmentStatus.StatusName
                }).ToList(),

                RecentReferrals = referrals
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new ReferralDto
                    {
                        ReferralId = r.ReferralId,
                        Specialty = r.SpecialtyRequest.SpecialtyName,
                        ReferralStatus = r.ReferralStatus.StatusName,
                        Urgency = r.UrgencyLevel.LevelName,
                        OriginFacility = r.OriginFacility!=null?r.OriginFacility.FacilityName:string.Empty,
                        DestinationFacility = r.DestinationFacility != null ? r.DestinationFacility.FacilityName: string.Empty,
                        CreatedAt = r.CreatedAt ?? DateTime.UtcNow
                    }).ToList()
            };
        }

        //==========================================================
        // REFERRALS
        //==========================================================
        public async Task<List<ReferralDto>> GetReferralsAsync(int userId)
        {
            var patient = await GetPatientEntityAsync(userId);

            return await _context.Referrals
                .Where(r => r.PatientId == patient.PatientId)
                .Include(r => r.ReferralStatus)
                .Include(r => r.SpecialtyRequest)
                .Include(r => r.UrgencyLevel)
                .Include(r => r.OriginFacility)
                .Include(r => r.DestinationFacility)
                .Select(r => new ReferralDto
                {
                    ReferralId = r.ReferralId,
                    Specialty = r.SpecialtyRequest.SpecialtyName,
                    ReferralStatus = r.ReferralStatus.StatusName,
                    Urgency = r.UrgencyLevel.LevelName,
                    OriginFacility = r.OriginFacility != null ? r.OriginFacility.FacilityName : string.Empty,
                    DestinationFacility = r.DestinationFacility != null ? r.DestinationFacility.FacilityName : string.Empty,
                    CreatedAt = System.DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<ReferralDetailsDto> GetReferralByIdAsync(int userId, int referralId)
        {
            var patient = await GetPatientEntityAsync(userId);

            var r = await _context.Referrals
                .Where(r => r.PatientId == patient.PatientId && r.ReferralId == referralId)
                .Include(r => r.ReferralStatus)
                .Include(r => r.SpecialtyRequest)
                .Include(r => r.UrgencyLevel)
                .Include(r => r.OriginFacility)
                .Include(r => r.DestinationFacility)
                .FirstOrDefaultAsync();

            if (r == null)
                throw new NotFoundException("Referral not found.");

            var appointment = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.ReferralId == r.ReferralId)
                .Include(a => a.Specialist)
                    .ThenInclude(s => s.User)
                .Include(a => a.Specialist)
                    .ThenInclude(s => s.Facility)
                .Include(a => a.AppointmentStatus)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .FirstOrDefaultAsync();

            return new ReferralDetailsDto
            {
                ReferralId = r.ReferralId,
                ReferralReason = r.ReferralReason,
                DiagnosisCode = r.DiagnosisCode ?? "",
                Specialty = r.SpecialtyRequest.SpecialtyName,
                ReferralStatus = r.ReferralStatus.StatusName,
                Urgency = r.UrgencyLevel.LevelName,
                OriginFacility = r.OriginFacility?.FacilityName ?? string.Empty,
                DestinationFacility = r.DestinationFacility?.FacilityName ?? string.Empty,
                CreatedAt = r.CreatedAt ?? DateTime.UtcNow,
                AppointmentId = appointment?.AppointmentId,
                AppointmentDate = appointment?.AppointmentDate,
                AppointmentTime = appointment?.AppointmentTime,
                AppointmentStatus = appointment?.AppointmentStatus.StatusName,
                SpecialistName = appointment == null
                    ? null
                    : appointment.Specialist.User.FirstName + " " + appointment.Specialist.User.LastName,
                SpecialistFacility = appointment?.Specialist.Facility.FacilityName
            };
        }

        public async Task<ReferralStatusDto> GetReferralStatusAsync(int userId, int referralId)
        {
            var patient = await GetPatientEntityAsync(userId);

            var r = await _context.Referrals
                .Where(r => r.PatientId == patient.PatientId && r.ReferralId == referralId)
                .Include(r => r.ReferralStatus)
                .FirstOrDefaultAsync();

            if (r == null)
                throw new NotFoundException("Referral not found.");

            return new ReferralStatusDto
            {
                ReferralId = r.ReferralId,
                ReferralStatus = r.ReferralStatus.StatusName,
                LastUpdated = System.DateTime.UtcNow
            };
        }

        //==========================================================
        // APPOINTMENTS
        //==========================================================
        public async Task<List<AppointmentDto>> GetAppointmentsAsync(int userId)
        {
            var patient = await GetPatientEntityAsync(userId);

            return await _context.Appointments
                .Where(a => a.PatientId == patient.PatientId)
                .Include(a => a.Specialist).ThenInclude(s => s.User)
                .Include(a => a.AppointmentStatus)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                    FacilityName = a.Specialist.Facility.FacilityName,
                    Specialty = a.Referral.SpecialtyRequest.SpecialtyName,
                    AppointmentStatus = a.AppointmentStatus.StatusName
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(int userId)
        {
            var patient = await GetPatientEntityAsync(userId);

            return await _context.Appointments
                .Where(a => a.PatientId == patient.PatientId &&
                            a.AppointmentDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Include(a => a.Specialist).ThenInclude(s => s.User)
                .Include(a => a.AppointmentStatus)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                    FacilityName = a.Specialist.Facility.FacilityName,
                    Specialty = a.Referral.SpecialtyRequest.SpecialtyName,
                    AppointmentStatus = a.AppointmentStatus.StatusName
                })
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetAppointmentHistoryAsync(int userId)
        {
            var patient = await GetPatientEntityAsync(userId);

            return await _context.Appointments
                .Where(a => a.PatientId == patient.PatientId &&
                            a.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
                .Include(a => a.Specialist).ThenInclude(s => s.User)
                .Include(a => a.AppointmentStatus)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                    FacilityName = a.Specialist.Facility.FacilityName,
                    Specialty = a.Referral.SpecialtyRequest.SpecialtyName,
                    AppointmentStatus = a.AppointmentStatus.StatusName
                })
                .ToListAsync();
        }

        public async Task<AppointmentDetailsDto> GetAppointmentByIdAsync(int userId, int appointmentId)
        {
            var patient = await GetPatientEntityAsync(userId);

            var a = await _context.Appointments
                .Where(a => a.PatientId == patient.PatientId && a.AppointmentId == appointmentId)
                .Include(a => a.Specialist).ThenInclude(s => s.User)
                .Include(a => a.Specialist).ThenInclude(s => s.Facility)
                .Include(a => a.AppointmentStatus)
                .Include(a => a.Referral).ThenInclude(r => r.SpecialtyRequest)
                .FirstOrDefaultAsync();

            if (a == null)
                throw new NotFoundException("Appointment not found.");

            return new AppointmentDetailsDto
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                FacilityName = a.Specialist.Facility.FacilityName,
                AppointmentStatus = a.AppointmentStatus.StatusName,
                Specialty = a.Referral.SpecialtyRequest.SpecialtyName
            };
        }
    }
}
