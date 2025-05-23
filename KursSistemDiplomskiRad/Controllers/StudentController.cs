using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IStudentRepository _studentRepository;
        public StudentController(DataContext dataContext, IStudentRepository studentRepository) {
            _dataContext = dataContext;
            _studentRepository = studentRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllStudenti")]
        public async Task<IActionResult> GetAllStudenti()
        {
            var studenti = await _studentRepository.GetAllStudentiAsync();
            return Ok(studenti);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetKurseviZaStudenta/{studentId}")]
        public async Task<IActionResult> GetKurseviZaStudenta(int studentId)
        {
            var kursevi = await _studentRepository.IspisKursevaZaStudenta(studentId);
            if(kursevi == null || !kursevi.Any())
            {
                return NotFound(kursevi);
            }
            return Ok(kursevi);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("GetStudentiNaKursu/{kursId}")]
        public async Task<IActionResult> GetStudentiNaKursu(int kursId)
        {
            if (User.IsInRole("Admin"))
            {
                await _studentRepository.IspisStudenataNaKursu(kursId);
            }

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);

            if(student == null)
            {
                return Unauthorized();
            }

            if (!prijavljen)
            {
                return Unauthorized("Niste prijavljeni na ovaj kurs");
            }

            var studentiNaKursu = await _studentRepository.IspisStudenataNaKursu(kursId);

            if(studentiNaKursu == null || !studentiNaKursu.Any())
            {
                return NotFound("Nema studenata na ovom kursu");
            }

            return Ok(studentiNaKursu);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DodajStudentaNaKurs/{studentId}/{kursId}")]
        public async Task<IActionResult> DodajStudentaNaKurs(int studentId, int kursId)
        {
            var rezultat = await _studentRepository.DodajStudentaNaKurs(studentId, kursId);
            if (!rezultat)
            {
                return BadRequest("Student je vec prijavljen na kursu ili podaci nisu ispravni");
            }

            return Ok("Student je uspjesno dodan na kurs");
        }

        [HttpDelete("UkloniStudentaSaKursa/{studentId}/{kursId}")]
        public async Task<IActionResult> UkloniStudentaSaKursa(int studentId, int kursId)
        {
            var rezultat = await _studentRepository.UkloniStudentaSaKursa(studentId, kursId);
            if (!rezultat)
            {
                return NotFound("Student nije prijavljen na kurs");
            }

            return Ok("Student je uspjesno uklonjen sa kursa");
        }
    }
}
