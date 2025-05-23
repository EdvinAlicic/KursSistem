using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
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
            var kursevi = await _kursRepository.GetAllKurseviAsync();

            if (User.IsInRole("Student"))
            {
                var aktivniKursevi = kursevi.Where(k => k.StatusKursa == 1).ToList();

                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

                if (student != null)
                {
                    foreach (var kurs in aktivniKursevi)
                    {
                        var prijavljen = await _dataContext.StudentKurs
                            .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kurs.Id);
                    }
                }

                return Ok(aktivniKursevi);
            }

            foreach(var kurs in kursevi)
            {
                kurs.ProsjecnaOcjena = await _dataContext.Set<KursOcjena>()
                    .Where(o => o.KursId == kurs.Id)
                    .Select(o => (float?)o.Ocjena)
                    .AverageAsync();
            }

            /*if (User.IsInRole("Student"))
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

                if (student != null)
                {
                    foreach (var kurs in kursevi)
                    {
                        var prijavljen = await _dataContext.StudentKurs
                            .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kurs.Id);

                        if (!prijavljen)
                        {
                            kurs.Lekcije = new List<LekcijaDto>();
                            kurs.Studenti = new List<StudentOnKursDto>();
                        }
                    }
                }
            }*/

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

            if (User.IsInRole("Student"))
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

                if(student != null)
                {
                    var prijavljen = await _dataContext.StudentKurs
                        .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kurs.Id);

                    if (!prijavljen)
                    {
                        kurs.Lekcije = new List<LekcijaDto>();
                        kurs.Studenti = new List<StudentOnKursDto>();
                    }
                }
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
        [HttpPost("{kursId}/ocjena")]
        public async Task<IActionResult> DodajOcjenu(int kursId, [FromBody] KursOcjenaDto kursOcjenaDto)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var dodanaOcjena = await _kursOcjenaRepository.DodajOcjenuAsync(student.Id, kursId, kursOcjenaDto);
            if (!dodanaOcjena)
            {
                return BadRequest("Niste prijavljeni na kurs ili ste ga vec ocijenili");
            }

            return Ok("Ocjena je uspjesno dodana");
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{kursId}/SveOcjene")]
        public async Task<IActionResult> GetOcjeneZaKurs(int kursId)
        {
            var ocjene = await _kursOcjenaRepository.GetOcjeneZaKursAsync(kursId);
            return Ok(ocjene);
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{kursId}/ocjena")]
        public async Task<IActionResult> GetProsjecnaOcjena(int kursId)
        {
            var prosjek = await _kursOcjenaRepository.GetProsjecnaOcjenaAsync(kursId);
            return Ok(prosjek ?? 0);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("{kursId}/prijava")]
        public async Task<IActionResult> PrijavaNaKurs(int kursId)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti
                .FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var rezultat = await _kursRepository.PrijavaNaKurs(student.Id, kursId);

            if(rezultat == "Uspjesno ste se prijavili na kurs")
            {
                return Ok(rezultat);
            }

            return BadRequest(rezultat);
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

        [HttpDelete("{kursId}/odjava")]
        public async Task<IActionResult> OdjavaSaKursa(int kursId)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if(student == null)
            {
                return Unauthorized();
            }

            var rezultat = await _kursRepository.OdjavaSaKursa(student.Id, kursId);

            if(rezultat == "Uspjesno ste se odjavili sa kursa")
            {
                return Ok(rezultat);
            }

            return NotFound(rezultat);
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
