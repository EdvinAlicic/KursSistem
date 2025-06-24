using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Extensions;
using KursSistemDiplomskiRad.Helpers;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/kursevi/{kursId}/[controller]")]
    [ApiController]
    public class KursOcjenaController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IKursOcjenaRepository _kursOcjenaRepository;
        private readonly StudentValidationHelper _studentValidationHelper;
        public KursOcjenaController(DataContext dataContext, IKursOcjenaRepository kursOcjenaRepository, StudentValidationHelper studentValidationHelper)
        {
            _dataContext = dataContext;
            _kursOcjenaRepository = kursOcjenaRepository;
            _studentValidationHelper = studentValidationHelper;
        }

        [Authorize(Roles = "Student, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetOcjeneZaKurs(int kursId)
        {
            var ocjene = await _kursOcjenaRepository.GetOcjeneZaKursAsync(kursId);
            return Ok(ocjene);
        }

        [Authorize(Roles = "Student, Admin")]
        [HttpGet("prosjek")]
        public async Task<IActionResult> GetProsjecnaOcjena(int kursId)
        {
            var prosjek = await _kursOcjenaRepository.GetProsjecnaOcjenaAsync(kursId);
            return Ok(prosjek);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> DodajOcjenu(int kursId, [FromBody] KursOcjenaCreateDto kursOcjenaDto)
        {
            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var ocjena = await _kursOcjenaRepository.DodajOcjenuAsync(validationResult.StudentId, kursId, kursOcjenaDto);
            return CreatedAtAction(nameof(GetOcjeneZaKurs), new { kursId }, ocjena);
        }

        [Authorize(Roles = "Student")]
        [HttpPatch]
        public async Task<IActionResult> UpdateOcjena(int kursId, [FromBody] KursOcjenaUpdateDto kursOcjenaUpdateDto)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
            {
                return Unauthorized();
            }

            var updated = await _kursOcjenaRepository.UpdateOcjenaAsync(student.Id, kursId, kursOcjenaUpdateDto);

            if (!updated)
            {
                return NotFound("Ocjena nije pronađena ili ne možete je ažurirati.");
            }

            return Ok();
        }

        [Authorize(Roles = "Student")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOcjena(int kursId)
        {
            var email = User.GetUserEmail();
            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
            {
                return Unauthorized();
            }
            var deleted = await _kursOcjenaRepository.DeleteOcjenaAsync(student.Id, kursId);

            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
