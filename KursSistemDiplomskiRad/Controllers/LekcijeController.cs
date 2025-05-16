using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class LekcijeController : ControllerBase
    {
        private readonly ILekcijeRepository _lekcijeRepository;
        public LekcijeController(ILekcijeRepository lekcijeRepository)
        {
            _lekcijeRepository = lekcijeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLekcijeAsync(int kursId)
        {
            var lekcije = await _lekcijeRepository.GetAllLekcijeAsync(kursId);
            return Ok(lekcije);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLekcijaByIdAsync(int kursId, int id)
        {
            var lekcija = await _lekcijeRepository.GetLekcijaByIdAsync(id, kursId);
            if(lekcija == null)
            {
                return NotFound();
            }
            return Ok(lekcija);
        }

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
