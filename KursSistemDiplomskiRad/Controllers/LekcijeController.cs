using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class LekcijeController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILekcijeRepository _lekcijeRepository;
        public LekcijeController(DataContext dataContext, ILekcijeRepository lekcijeRepository)
        {
            _dataContext = dataContext;
            _lekcijeRepository = lekcijeRepository;
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllLekcijeAsync(int kursId)
        {
            // Admin vidi sve
            if (User.IsInRole("Admin"))
                return Ok(await _lekcijeRepository.GetAllLekcijeAsync(kursId));

            // Student vidi samo ako je prijavljen
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
                return Unauthorized();

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);

            if (!prijavljen)
                return StatusCode(StatusCodes.Status403Forbidden, "Niste prijavljeni na ovaj kurs.");

            return Ok(await _lekcijeRepository.GetAllLekcijeAsync(kursId));
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLekcijaByIdAsync(int kursId, int id)
        {
            // Admin vidi sve
            if (User.IsInRole("Admin"))
                return Ok(await _lekcijeRepository.GetLekcijaByIdAsync(id, kursId));

            // Student vidi samo ako je prijavljen
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
                return Unauthorized();

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);

            if (!prijavljen)
                return StatusCode(StatusCodes.Status403Forbidden, "Niste prijavljeni na ovaj kurs.");

            var lekcija = await _lekcijeRepository.GetLekcijaByIdAsync(id, kursId);
            if (lekcija == null)
                return NotFound();

            return Ok(lekcija);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddLekcijaAsync(int kursId, [FromForm] LekcijaCreateDto lekcijaDto)
        {
            if (lekcijaDto.MedijskiSadrzaj == null || lekcijaDto.MedijskiSadrzaj.Length == 0)
                return BadRequest("Medijski sadržaj nije dostupan");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(lekcijaDto.MedijskiSadrzaj.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await lekcijaDto.MedijskiSadrzaj.CopyToAsync(stream);
            }

            // DTO za repozitorijum (ili entitet) treba MedijskiSadrzaj kao string (putanja)
            var lekcijaZaRepo = new LekcijaCreateDto
            {
                Naziv = lekcijaDto.Naziv,
                Opis = lekcijaDto.Opis,
                MedijskiSadrzaj = null // Ne koristi više, ali možeš napraviti novi DTO/entitet
            };

            // Ručno kreiraj entitet
            var lekcijaEntity = new Lekcije
            {
                Naziv = lekcijaDto.Naziv,
                Opis = lekcijaDto.Opis,
                MedijskiSadrzaj = "/uploads/" + fileName,
                KursId = kursId
            };

            _dataContext.Lekcije.Add(lekcijaEntity);
            await _dataContext.SaveChangesAsync();

            // Mapiraj u DTO za ispis
            var lekcijaZaIspis = new LekcijaDto
            {
                Id = lekcijaEntity.Id,
                Naziv = lekcijaEntity.Naziv,
                Opis = lekcijaEntity.Opis,
                MedijskiSadrzaj = lekcijaEntity.MedijskiSadrzaj,
                KursId = lekcijaEntity.KursId
            };

            return Ok(lekcijaZaIspis);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLekcijaAsync(int id, int kursId, [FromForm] LekcijaZaUpdateDto lekcijaDto)
        {
            var lekcija = await _dataContext.Lekcije.FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if (lekcija == null)
                return NotFound();

            lekcija.Naziv = lekcijaDto.Naziv;
            lekcija.Opis = lekcijaDto.Opis;

            if (lekcijaDto.MedijskiSadrzaj != null && lekcijaDto.MedijskiSadrzaj.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(lekcijaDto.MedijskiSadrzaj.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lekcijaDto.MedijskiSadrzaj.CopyToAsync(stream);
                }

                lekcija.MedijskiSadrzaj = "/uploads/" + fileName;
            }

            await _dataContext.SaveChangesAsync();

            var lekcijaZaIspis = new LekcijaDto
            {
                Id = lekcija.Id,
                Naziv = lekcija.Naziv,
                Opis = lekcija.Opis,
                MedijskiSadrzaj = lekcija.MedijskiSadrzaj,
                KursId = lekcija.KursId
            };

            return Ok(lekcijaZaIspis);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLekcijaAsync(int id, int kursId)
        {
            var result = await _lekcijeRepository.DeleteLekcijaAsync(id, kursId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
