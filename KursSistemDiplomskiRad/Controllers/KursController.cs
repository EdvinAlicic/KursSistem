using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KursController : ControllerBase
    {
        private readonly IKursRepository _kursRepository;
        public KursController(IKursRepository kursRepository)
        {
            _kursRepository = kursRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKursevi()
        {
            var kursevi = await _kursRepository.GetAllKurseviAsync();
            return Ok(kursevi);
        }

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
    }
}
