using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KursController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IKursRepository _kursRepository;
        public KursController(DataContext dataContext, IKursRepository kursRepository)
        {
            _dataContext = dataContext;
            _kursRepository = kursRepository;
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllKursevi()
        {
            var kursevi = await _kursRepository.GetAllKurseviAsync();
            return Ok(kursevi);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("id")]
        public async Task<IActionResult> GetKursById(int id)
        {
            var kurs = await _kursRepository.GetKursByIdAsync(id);
            if(kurs == null)
            {
                return NotFound();
            }
            return Ok(kurs);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddKurs([FromBody] KursCreateDto kurs)
        {
            var createdKurs = await _kursRepository.AddKursAsync(kurs);
            if(createdKurs == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetKursById), new { id = createdKurs.Id }, createdKurs);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("{kursId}/prijava")]
        public async Task<IActionResult> PrijavaNaKurs(int kursId)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var postoji = await _dataContext.StudentKurs.AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);

            if (postoji)
            {
                return BadRequest("Vec ste prijavljeni na ovaj kurs");
            }

            var prijava = new StudentKurs
            {
                StudentId = student.Id,
                KursId = kursId,
                DatumPrijave = DateTime.Now,
                StatusPrijave = "Aktivan"
            };
            await _dataContext.StudentKurs.AddAsync(prijava);
            await _dataContext.SaveChangesAsync();
            return Ok("Uspjesno ste se prijavili na kurs");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("id")]
        public async Task<IActionResult> UpdateKurs(int id, [FromBody] KursUpdateDto kurs)
        {
            var updatedKurs = await _kursRepository.UpdateKursAsync(id, kurs);
            if(updatedKurs == null)
            {
                return NotFound();
            }
            return Ok(updatedKurs);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteKursAsync(int id)
        {
            var deletedKurs = await _kursRepository.DeleteKursAsync(id);
            if(deletedKurs == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
