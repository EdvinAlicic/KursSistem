using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
        private readonly IKursOcjenaRepository _kursOcjenaRepository;
        public KursController(DataContext dataContext, IKursRepository kursRepository, IKursOcjenaRepository kursOcjenaRepository)
        {
            _dataContext = dataContext;
            _kursRepository = kursRepository;
            _kursOcjenaRepository = kursOcjenaRepository;
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllKursevi()
        {
            var aktivniKursevi = await _kursRepository.GetAllKurseviAsync();

            foreach (var kurs in aktivniKursevi)
            {
                kurs.ProsjecnaOcjena = await _dataContext.Set<KursOcjena>()
                    .Where(o => o.KursId == kurs.Id)
                    .Select(o => (float?)o.Ocjena)
                    .AverageAsync();
            }

            return Ok(aktivniKursevi);
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

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("neaktivni-kursevi")]
        public async Task<IActionResult> GetNeaktivniKursevi()
        {
            if (User.IsInRole("Admin"))
            {
                var neaktivniKursevi = await _kursRepository.GetAllNeaktivneKurseve();
                return Ok(neaktivniKursevi);
            }

            if (User.IsInRole("Student"))
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
                if(student != null)
                {
                    var neaktivniKursevi = await _kursRepository.GetNeaktivneKurseveZaStudenta(student.Id);
                    return Ok(neaktivniKursevi);
                }
            }

            return Ok();
        }

        [HttpGet("pretraga")]
        public async Task<IActionResult> SearchKursevi([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Unesite termin za pretragu");
            }

            var kursevi = await _kursRepository.SearchKurseviAsync(searchTerm);
            if (!kursevi.Any())
            {
                return NotFound("Nema kurseva koji odgovaraju pretrazi");
            }

            return Ok(kursevi);
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
        [HttpPost("{kursId}/prijava-na-kurs")]
        public async Task<IActionResult> PrijavaNaKurs(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti
                .FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var rezultat = await _kursRepository.PrijavaNaKurs(student.Id, kursId);

            if(rezultat)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("id")]
        public async Task<IActionResult> UpdateKurs(int id, [FromBody] KursUpdateDto kurs)
        {
            var updatedKurs = await _kursRepository.UpdateKursAsync(id, kurs);
            if(updatedKurs == null)
            {
                return NotFound();
            }
            return Ok(updatedKurs);
        }

        [Authorize(Roles = "Student")]
        [HttpDelete("{kursId}/odjava-sa-kursa")]
        public async Task<IActionResult> OdjavaSaKursa(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var rezultat = await _kursRepository.OdjavaSaKursa(student.Id, kursId);

            if(rezultat)
            {
                return Ok();
            }

            return NotFound();
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
