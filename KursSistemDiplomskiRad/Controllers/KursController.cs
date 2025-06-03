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
        [HttpGet("NeaktivniKursevi")]
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
        [HttpPost("{kursId}/AddOcjena")]
        public async Task<IActionResult> DodajOcjenu(int kursId, [FromBody] KursOcjenaCreateDto kursOcjenaCreateDto)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var dodanaOcjena = await _kursOcjenaRepository.DodajOcjenuAsync(student.Id, kursId, kursOcjenaCreateDto);
            if (dodanaOcjena == null)
            {
                return BadRequest("Niste prijavljeni na kurs ili ste ga vec ocijenili");
            }

            return Ok("Ocjena je uspjesno dodana");
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{kursId}/GetOcjene")]
        public async Task<IActionResult> GetOcjeneZaKurs(int kursId)
        {
            var ocjene = await _kursOcjenaRepository.GetOcjeneZaKursAsync(kursId);
            return Ok(ocjene);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{kursId}/GetProsjecnaOcjena")]
        public async Task<IActionResult> GetProsjecnaOcjena(int kursId)
        {
            var prosjek = await _kursOcjenaRepository.GetProsjecnaOcjenaAsync(kursId);
            return Ok(prosjek ?? 0);
        }

        [Authorize(Roles = "Student")]
        [HttpPatch("{kursId}/UpdateOcjena")]
        public async Task<IActionResult> UpdateOcjena(int kursId, [FromBody] KursOcjenaUpdateDto kursOcjenaUpdateDto)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var updatedOcjena = await _kursOcjenaRepository.UpdateOcjenaAsync(student.Id, kursId, kursOcjenaUpdateDto);

            if (!updatedOcjena)
            {
                return NotFound();
            }

            var prosjek = await _kursOcjenaRepository.GetProsjecnaOcjenaAsync(kursId);

            return Ok(new
            {
                message = "Ocjena je azurirana",
                prosjecnaOcjena = prosjek ?? 0
            });
        }

        [Authorize(Roles = "Student")]
        [HttpDelete("{kursId}/DeleteOcjena")]
        public async Task<IActionResult> DeleteOcjena(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var deletedOcjena = await _kursOcjenaRepository.DeleteOcjenaAsync(student.Id, kursId);
            if (!deletedOcjena)
            {
                return NotFound();
            }

            var prosjek = await _kursOcjenaRepository.GetProsjecnaOcjenaAsync(kursId);

            return Ok(new
            {
                message = "Ocjena je uspjesno obrisana",
                prosjecnaOcjena = prosjek ?? 0
            });
        }

        [Authorize(Roles = "Student")]
        [HttpPost("{kursId}/prijava")]
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
        [HttpDelete("{kursId}/odjava")]
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
