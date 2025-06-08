using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class LekcijeProgressController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IStudentLekcijaProgressRepository _studentLekcijaProgressRepository;
        public LekcijeProgressController(DataContext dataContext, IStudentLekcijaProgressRepository studentLekcijaProgressRepository)
        {
            _dataContext = dataContext;
            _studentLekcijaProgressRepository = studentLekcijaProgressRepository;
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
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
        [HttpGet("lekcije")]
        public async Task<IActionResult> GetZavrseneLekcije(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null)
            {
                return Unauthorized();
            }

            var progress = await _studentLekcijaProgressRepository.GetProgressZaLekcije(student.Id, kursId);
            var zavrseneLekcije = progress.Where(p => p.JeZavrsena).ToList();

            return Ok(zavrseneLekcije);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("zavrsi")]
        public async Task<IActionResult> ZavrsiLekciju(int kursId, int id)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null)
            {
                return Unauthorized();
            }

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);
            if (!prijavljen)
            {
                return Forbid();
            }

            var lekcija = await _dataContext.Lekcije
                .FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if (lekcija == null)
            {
                return BadRequest();
            }

            var result = await _studentLekcijaProgressRepository.OznaciLekcijuKaoZavrsenu(student.Id, kursId, id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize(Roles = "Student")]
        [HttpPatch("opozovi-zavrsetak")]
        public async Task<IActionResult> OpozoviZavrsenuLekciju(int kursId, int id)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null)
            {
                return Unauthorized();
            }

            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);
            if (!prijavljen)
            {
                return Forbid();
            }

            var lekcija = await _dataContext.Lekcije
                .FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if (lekcija == null)
            {
                return BadRequest();
            }

            var result = await _studentLekcijaProgressRepository.OpozoviZavrsenuLekciju(student.Id, kursId, id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
