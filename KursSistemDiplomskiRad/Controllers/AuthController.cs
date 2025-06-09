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
        private readonly IConfiguration _config;
        public AuthController(DataContext dataContext, IConfiguration config)
        {
            _dataContext = dataContext;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            if (!IsValidEmail(registerDto.Email))
            {
                return BadRequest("Neispravan Email");
            }

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
                Role = "Student",
                DatumRegistracije = DateTime.Now
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

            var token = GenerateJwtToken(user.Email, role);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.Now.AddDays(7),
                IsRevoked = false,
                StudentId = user.Id
            };

            user.ZadnjaPrijava = DateTime.Now;
            _dataContext.RefreshTokens.Add(refreshTokenEntity);
            await _dataContext.SaveChangesAsync();

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddDays(7),
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { token, role });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

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
                Expires = DateTime.Now.AddDays(7),
                IsRevoked = false,
                StudentId = user.Id
            };

            _dataContext.RefreshTokens.Add(newTokenEntity);
            await _dataContext.SaveChangesAsync();

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddDays(7),
                SameSite = SameSiteMode.Strict,
            });

            return Ok(new { token = newJwt, role });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
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
