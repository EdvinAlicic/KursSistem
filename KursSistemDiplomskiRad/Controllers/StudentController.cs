using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Helpers;
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
        private readonly StudentValidationHelper _studentValidationHelper;
        public StudentController(DataContext dataContext, IStudentRepository studentRepository, StudentValidationHelper studentValidationHelper) {
            _dataContext = dataContext;
            _studentRepository = studentRepository;
            _studentValidationHelper = studentValidationHelper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllStudenti()
        {
            var studenti = await _studentRepository.GetAllStudentiAsync();
            return Ok(studenti);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{studentId}")]
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

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("kursevi/{studentId}")]
        public async Task<IActionResult> GetKurseviZaStudenta(int studentId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var kursevi = await _studentRepository.IspisKursevaZaStudenta(studentId);
            if(kursevi == null || !kursevi.Any())
            {
                return NotFound(kursevi);
            }

            return Ok(kursevi);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("studenti-na-kursu/{kursId}")]
        public async Task<IActionResult> GetStudentiNaKursu(int kursId)
        {
            var studentiNaKursu = await _studentRepository.IspisStudenataNaKursu(kursId);

            if (User.IsInRole("Admin"))
            {
                return Ok(studentiNaKursu);
            }

            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            if (studentiNaKursu == null)
            {
                return NotFound("Nema studenata na ovom kursu");
            }

            return Ok(studentiNaKursu);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpPatch("{studentId}")]
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
        [HttpPost("dodaj/{studentId}/{kursId}")]
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
        [HttpDelete("ukloni/{studentId}/{kursId}")]
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
        [HttpDelete("obrisi/{studentId}")]
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
