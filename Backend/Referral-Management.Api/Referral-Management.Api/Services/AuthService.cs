using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs.Auth;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Referral_Management.Api.Exceptions;

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
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var email = dto.Email.Trim().ToLower();

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                throw new BadRequestException("Email already exists.");
            }

            var facilityExists = await _context.Facilities
                .AnyAsync(f => f.FacilityId == dto.FacilityId);

            if (!facilityExists)
            {
                throw new BadRequestException("Selected facility does not exist.");
            }

            var roleExists = await _context.Roles
                .AnyAsync(r => r.RoleId == dto.RoleId);

            if (!roleExists)
            {
                throw new BadRequestException("Selected role does not exist.");
            }

            var user = new User
            {
                RoleId = dto.RoleId,
                FacilityId = dto.FacilityId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = email,
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
                case 1:
                    await CreateAdminAsync(user.UserId);
                    break;

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
                    throw new BadRequestException("Invalid role selected.");
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


    public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO) {

        var email = loginDTO.Email?.Trim().ToLower() ?? string.Empty;

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.Status)
        {
            throw new UnauthorizedException("Account is inactive.");
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password,user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var token = await GenerateJwtToken(user);

        return new LoginResponseDTO
        {
            UserId = user.UserId,
            Email = user.Email,
            RoleId = user.RoleId,
            Token = token
        };
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"];

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("FacilityId", user.FacilityId.ToString())
        };

        switch (user.RoleId)
        {
            case 1: // Admin
                {
                    var admin = await _context.Admins
                        .FirstOrDefaultAsync(a => a.UserId == user.UserId);

                    if (admin != null)
                    {
                        claims.Add(
                            new Claim("AdminId", admin.AdminId.ToString()));
                    }

                    break;
                }

            case 2: // Coordinator
                {
                    var coordinator = await _context.ReferralCoordinators
                        .FirstOrDefaultAsync(c => c.UserId == user.UserId);

                    if (coordinator != null)
                    {
                        claims.Add(
                            new Claim("ReferralCoordinatorId",
                                coordinator.ReferralCoordinatorId.ToString()));
                    }

                    break;
                }

            case 3: // Patient
                {
                    var patient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.UserId == user.UserId);

                    if (patient != null)
                    {
                        claims.Add(
                            new Claim("PatientId",
                                patient.PatientId.ToString()));
                    }

                    break;
                }

            case 4: // Specialist
                {
                    var specialist = await _context.Specialists
                        .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                    if (specialist != null)
                    {
                        claims.Add(
                            new Claim("SpecialistId",
                                specialist.SpecialistId.ToString()));
                    }

                    break;
                }
        }

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
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

    private async Task CreateAdminAsync(int userId)
    {
        var admin = new Admin
        {
            UserId = userId
        };

        _context.Admins.Add(admin);

        await _context.SaveChangesAsync();

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
            throw new BadRequestException("MRN is required.");

        if (dto.Dob == null)
            throw new BadRequestException("DOB is required.");

        if (string.IsNullOrWhiteSpace(dto.Gender))
            throw new BadRequestException("Gender is required.");

        if (string.IsNullOrWhiteSpace(dto.PatientAddress))
            throw new BadRequestException("Patient address is required.");

        var mrnExists = await _context.Patients
            .AnyAsync(p => p.Mrn == dto.Mrn);

        if (mrnExists)
        {
            throw new BadRequestException("MRN already exists.");
        }

        if (dto.Dob > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BadRequestException("DOB cannot be in the future.");
        }

        var patient = new Patient
        {
            UserId = userId,
            Mrn = dto.Mrn,
            Dob = dto.Dob.Value,
            Gender = dto.Gender,
            InsuranceProviderName = dto.InsuranceProviderName,
            InsuranceStatus = dto.InsuranceStatus,
            PatientAddress = dto.PatientAddress,
            PrimaryFacilityId = dto.FacilityId,
            Status = true
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    private async Task CreateSpecialistAsync(int userId, RegisterUserDTO dto)
    {
        if (dto.Specialties == null || !dto.Specialties.Any())
        {
            throw new BadRequestException("At least one specialty is required.");
        }

        var duplicateSpecialties = dto.Specialties
            .GroupBy(s => s.SpecialtyId)
            .Any(g => g.Count() > 1);

        if (duplicateSpecialties)
        {
            throw new BadRequestException("Duplicate specialties are not allowed.");
        }

        var primaryCount = dto.Specialties
            .Count(s => s.IsPrimary);

        if (primaryCount != 1)
        {
            throw new BadRequestException("Exactly one primary specialty is required.");
        }

        foreach (var specialty in dto.Specialties)
        {
            var specialtyExists = await _context.Specialties
                    .AnyAsync(s => s.SpecialtyId == specialty.SpecialtyId);

            if (!specialtyExists)
            {
                throw new BadRequestException($"Specialty {specialty.SpecialtyId} does not exist.");
            }
        }

        if (dto.ShiftBlockId.HasValue)
        {
            var shiftExists = await _context.ShiftBlocks
                    .AnyAsync(s =>
                        s.ShiftBlockId ==
                        dto.ShiftBlockId);

            if (!shiftExists)
            {
                throw new BadRequestException("Selected shift block does not exist.");
            }
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
            var specialistSpecialty = new SpecialistSpeciality
                {
                    SpecialistId = specialist.SpecialistId,
                    SpecialtyId = specialty.SpecialtyId,
                    IsPrimary = specialty.IsPrimary
                };

            _context.SpecialistSpecialities.Add(specialistSpecialty);
        }

        await _context.SaveChangesAsync();
    }
}