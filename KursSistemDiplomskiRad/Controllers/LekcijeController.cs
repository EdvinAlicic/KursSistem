using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Extensions;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class LekcijeController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILekcijeRepository _lekcijeRepository;
        private readonly IStudentLekcijaProgressRepository _studentLekcijaProgressRepository;
        public LekcijeController(DataContext dataContext, ILekcijeRepository lekcijeRepository, IStudentLekcijaProgressRepository studentLekcijaProgressRepository)
        {
            _dataContext = dataContext;
            _lekcijeRepository = lekcijeRepository;
            _studentLekcijaProgressRepository = studentLekcijaProgressRepository;
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllLekcijeAsync(int kursId)
        {
            // Admin vidi sve
            if (User.IsInRole("Admin"))
                return Ok(await _lekcijeRepository.GetAllLekcijeAsync(kursId));

            // Student vidi samo ako je prijavljen
            var email = User.GetUserEmail();
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
            var email = User.GetUserEmail();
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

        [Authorize(Roles = "Student")]
        [HttpGet("progress")]
        public async Task<IActionResult> GetProgress(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if(student == null)
            {
                return Unauthorized();
            }

            var progress = await _studentLekcijaProgressRepository.GetKursProgressZaStudenta(student.Id, kursId);
            return Ok(progress);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("progress/lekcije")]
        public async Task<IActionResult> GetZavrseneLekcije(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if(student == null)
            {
                return Unauthorized();
            }

            var progress = await _studentLekcijaProgressRepository.GetProgressZaLekcije(student.Id, kursId);
            var zavrseneLekcije = progress.Where(p => p.JeZavrsena).ToList();

            return Ok(zavrseneLekcije);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddLekcijaAsync(int kursId, [FromForm] LekcijaCreateDto lekcijaDto)
        {
            var lekcija = await _lekcijeRepository.AddLekcijaAsync(lekcijaDto, kursId);
            if (lekcija == null)
                return BadRequest();

            return Ok(lekcija);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("{id}/zavrsi")]
        public async Task<IActionResult> ZavrsiLekciju(int kursId, int id)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if(student == null)
            {
                return Unauthorized();
            }

            var result = await _studentLekcijaProgressRepository.OznaciLekcijuKaoZavrsenu(student.Id, kursId, id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateLekcijaAsync(int id, int kursId, [FromForm] LekcijaZaUpdateDto lekcijaDto)
        {
            var lekcija = await _lekcijeRepository.UpdateLekcijaAsync(id, lekcijaDto, kursId);
            if (lekcija == null)
                return NotFound();

            return Ok(lekcija);
        }

        [Authorize(Roles = "Student")]
        [HttpPatch("{id}/OpozoviZavrsetak")]
        public async Task<IActionResult> OpozoviZavrsenuLekciju(int kursId, int id)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if(student == null)
            {
                return Unauthorized();
            }

            var result = await _studentLekcijaProgressRepository.OpozoviZavrsenuLekciju(student.Id, kursId, id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
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
