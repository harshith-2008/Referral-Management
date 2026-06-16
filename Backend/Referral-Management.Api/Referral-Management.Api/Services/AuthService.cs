using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs.Auth;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Referral_Management.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterUserDTO dto)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            var user = new User
            {
                RoleId = dto.RoleId,
                FacilityId = dto.FacilityId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            switch (dto.RoleId)
            {
                case 2:
                    await CreateReferralCoordinatorAsync(user.UserId, dto);
                    break;

                case 3:
                    await CreatePatientAsync(user.UserId, dto);
                    break;

                case 4:
                    await CreateSpecialistAsync(user.UserId, dto);
                    break;

                default:
                    throw new Exception("Invalid role selected.");
            }

            await transaction.CommitAsync();

            return new RegisterResponseDTO
            {
                UserId = user.UserId,
                Message = "User registered successfully."
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<LoginResponseDTO> LoginAsync(
    LoginDTO loginDTO)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == loginDTO.Email);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                loginDTO.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new Exception("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDTO
        {
            UserId = user.UserId,
            Email = user.Email,
            RoleId = user.RoleId,
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"];

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier,
            user.UserId.ToString()),

        new Claim(ClaimTypes.Email,
            user.Email),

        new Claim(ClaimTypes.Role,
            user.RoleId.ToString())
    };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(
                        _configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private async Task CreateReferralCoordinatorAsync(int userId, RegisterUserDTO dto)
    {
        var coordinator = new ReferralCoordinator
        {
            UserId = userId,
            FacilityId = dto.FacilityId
        };

        _context.ReferralCoordinators.Add(coordinator);

        await _context.SaveChangesAsync();
    }

    private async Task CreatePatientAsync( int userId, RegisterUserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Mrn))
            throw new Exception("MRN is required.");
        if (dto.Dob == null)
            throw new Exception("DOB is required.");
        if (string.IsNullOrWhiteSpace(dto.Gender))
            throw new Exception("Gender is required.");
        if (string.IsNullOrWhiteSpace(dto.PatientAddress))
            throw new Exception("Patient address is required.");

        var patient = new Patient
        {
            UserId = userId,
            Mrn = dto.Mrn,
            Dob = dto.Dob.Value,
            Gender = dto.Gender,
            InsuranceProviderName =
                dto.InsuranceProviderName,
            InsuranceStatus =
                dto.InsuranceStatus,
            PatientAddress =
                dto.PatientAddress,
            PrimaryFacilityId =
                dto.FacilityId,
            Status = true
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    private async Task CreateSpecialistAsync(int userId, RegisterUserDTO dto)
    {
        if (dto.Specialties == null || !dto.Specialties.Any())
        {
            throw new Exception(
                "At least one specialty is required.");
        }

        var primaryCount = dto.Specialties
            .Count(s => s.IsPrimary);

        if (primaryCount != 1)
        {
            throw new Exception(
                "Exactly one primary specialty is required.");
        }

        var specialist = new Specialist
        {
            UserId = userId,
            FacilityId = dto.FacilityId,
            ShiftBlockId = dto.ShiftBlockId,
            Status = true
        };

        _context.Specialists.Add(specialist);

        await _context.SaveChangesAsync();

        foreach (var specialty in dto.Specialties)
        {
            var specialistSpecialty =
                new SpecialistSpeciality
                {
                    SpecialistId =
                        specialist.SpecialistId,
                    SpecialtyId =
                        specialty.SpecialtyId,
                    IsPrimary =
                        specialty.IsPrimary
                };

            _context.SpecialistSpecialities
                .Add(specialistSpecialty);
        }

        await _context.SaveChangesAsync();
    }
}