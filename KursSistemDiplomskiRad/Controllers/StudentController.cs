using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("GetStudentById/{studentId}")]
        public async Task<IActionResult> GetStudentById(int studentId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            if(student.Id != studentId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var studentDto = await _studentRepository.GetStudentById(studentId);
            if(studentDto == null)
            {
                return NotFound();
            }

            return Ok(studentDto);
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
            var studentiNaKursu = await _studentRepository.IspisStudenataNaKursu(kursId);

            if (User.IsInRole("Admin"))
            {
                return Ok(studentiNaKursu);
            }

            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);

            if (!prijavljen)
            {
                return Unauthorized("Niste prijavljeni na ovaj kurs");
            }

            if(studentiNaKursu == null)
            {
                return NotFound("Nema studenata na ovom kursu");
            }

            return Ok(studentiNaKursu);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpPatch("UpdateStudenta/{studentId}")]
        public async Task<IActionResult> UpdateStudenta(int studentId, [FromBody] StudentUpdateDto studentUpdateDto)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            if(student.Id != studentId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _studentRepository.UpdateStudentAsync(studentId, studentUpdateDto);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin, Student")]
        [HttpDelete("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            if(student.Id != studentId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _studentRepository.DeleteStudentAsync(studentId);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
