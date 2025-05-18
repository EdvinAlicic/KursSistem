using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> AddLekcijaAsync(int kursId, [FromBody] LekcijaCreateDto lekcijaDto)
        {
            var createdLekcija = await _lekcijeRepository.AddLekcijaAsync(lekcijaDto, kursId);
            if(createdLekcija == null)
            {
                return BadRequest();
            }
            return Ok(createdLekcija);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLekcijaAsync(int id, int kursId, [FromBody] LekcijaDto lekcijaDto)
        {
            var updatedLekcija = await _lekcijeRepository.UpdateLekcijaAsync(id, lekcijaDto, kursId);
            if(updatedLekcija == null)
            {
                return NotFound();
            }
            return Ok(updatedLekcija);
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
