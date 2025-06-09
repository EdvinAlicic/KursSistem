using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Helpers;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class LekcijeController : ControllerBase
    {
        private readonly ILekcijeRepository _lekcijeRepository;
        private readonly StudentValidationHelper _studentValidationHelper;
        public LekcijeController(ILekcijeRepository lekcijeRepository, StudentValidationHelper studentValidationHelper)
        {
            _lekcijeRepository = lekcijeRepository;
            _studentValidationHelper = studentValidationHelper;
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet]
        public async Task<IActionResult> GetAllLekcijeAsync(int kursId)
        {
            if (User.IsInRole("Admin"))
                return Ok(await _lekcijeRepository.GetAllLekcijeAsync(kursId));

            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            return Ok(await _lekcijeRepository.GetAllLekcijeAsync(kursId));
        }

        [Authorize(Roles = "Admin, Student")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLekcijaByIdAsync(int kursId, int id)
        {
            if (User.IsInRole("Admin"))
                return Ok(await _lekcijeRepository.GetLekcijaByIdAsync(id, kursId));

            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var lekcija = await _lekcijeRepository.GetLekcijaByIdAsync(id, kursId);
            if (lekcija == null)
                return NotFound();

            return Ok(lekcija);
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

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateLekcijaAsync(int id, int kursId, [FromForm] LekcijaZaUpdateDto lekcijaDto)
        {
            var lekcija = await _lekcijeRepository.UpdateLekcijaAsync(id, lekcijaDto, kursId);
            if (lekcija == null)
                return NotFound();

            return Ok(lekcija);
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
