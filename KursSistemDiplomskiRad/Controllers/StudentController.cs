using AutoMapper;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository) {
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
