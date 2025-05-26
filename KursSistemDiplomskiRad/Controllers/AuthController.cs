using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AuthController(DataContext dataContext, IConfiguration config)
        {
            _dataContext = dataContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if(await _dataContext.Studenti.AnyAsync(s => s.Email == registerDto.Email))
            {
                return BadRequest("Email vec postoji");
            }

            var student = new Student
            {
                Ime = registerDto.Ime,
                Prezime = registerDto.Prezime,
                Email = registerDto.Email,
                Telefon = registerDto.Telefon,
                Adresa = registerDto.Adresa,
                Role = "Student"
            };

            var hasher = new PasswordHasher<Student>();
            student.Password = hasher.HashPassword(student, registerDto.Password);

            await _dataContext.Studenti.AddAsync(student);
            await _dataContext.SaveChangesAsync();
            return Ok("Registracija uspjesna");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _dataContext.Studenti
                .FirstOrDefaultAsync(s => s.Email == loginDto.Email);

            if (user == null)
                return Unauthorized("Pogresan email ili lozinka");

            var hasher = new PasswordHasher<Student>();
            var result = hasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Pogresan email ili lozinka");

            // Role iz baze (Admin ili Student)
            var role = user.Role ?? "Student";

            var token = GenerateJwtToken(loginDto.Email, role);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                StudentId = user.Id
            };

            _dataContext.RefreshTokens.Add(refreshTokenEntity);
            await _dataContext.SaveChangesAsync();

            return Ok(new { token, role, refreshToken });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var tokenEntity = await _dataContext.RefreshTokens
                .Include(rt => rt.Student)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

            if(tokenEntity == null)
            {
                return Unauthorized("Neispravan ili istekao refresh token");
            }

            var user = tokenEntity.Student;
            var role = user.Role ?? "Student";
            var newJwt = GenerateJwtToken(user.Email, role);

            tokenEntity.IsRevoked = true;
            var newRefreshToken = GenerateRefreshToken();
            var newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                StudentId = user.Id
            };

            _dataContext.RefreshTokens.Add(newTokenEntity);
            await _dataContext.SaveChangesAsync();

            return Ok(new { token = newJwt, refreshToken = newRefreshToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var tokenEntity = await _dataContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if(tokenEntity == null)
            {
                return NotFound("Refresh token ne postoji ili je vec opozvan");
            }

            tokenEntity.IsRevoked = true;
            await _dataContext.SaveChangesAsync();

            return Ok("Uspjesno ste se odjavili");
        }


        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KursSistemSecretKey1234567890!@#$%^&*()"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            System.Security.Cryptography.RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
