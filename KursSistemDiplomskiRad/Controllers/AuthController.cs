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

        private const string AdminEmail = "admin@gmail.com";
        private const string AdminPassword = "admin123";

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
                Adresa = registerDto.Adresa
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
            string role;
            if(loginDto.Email == AdminEmail && loginDto.Password == AdminPassword)
            {
                role = "Admin";
            }
            else
            {
                var student = await _dataContext.Studenti
                    .FirstOrDefaultAsync(s => s.Email == loginDto.Email);
                if(student == null)
                {
                    return Unauthorized("Pogresan email ili lozinka");
                }

                var hasher = new PasswordHasher<Student>();
                var result = hasher.VerifyHashedPassword(student, student.Password, loginDto.Password);
                if(result != PasswordVerificationResult.Success)
                {
                    return Unauthorized("Pogresan email ili lozinka");
                }

                role = "Student";
            }

            var token = GenerateJwtToken(loginDto.Email, role);
            return Ok(new { token, role });
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
    }
}
