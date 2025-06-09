using KursSistemDiplomskiRad.Data;
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
    public class LekcijeProgressController : ControllerBase
    {
        private readonly IStudentLekcijaProgressRepository _studentLekcijaProgressRepository;
        private readonly StudentValidationHelper _studentValidationHelper;
        public LekcijeProgressController(DataContext dataContext, IStudentLekcijaProgressRepository studentLekcijaProgressRepository, StudentValidationHelper studentValidationHelper)
        {
            _studentLekcijaProgressRepository = studentLekcijaProgressRepository;
            _studentValidationHelper = studentValidationHelper;
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> GetProgress(int kursId)
        {
            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var progress = await _studentLekcijaProgressRepository.GetKursProgressZaStudenta(validationResult.StudentId, kursId);
            return Ok(progress);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("lekcije")]
        public async Task<IActionResult> GetZavrseneLekcije(int kursId)
        {
            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var progress = await _studentLekcijaProgressRepository.GetProgressZaLekcije(validationResult.StudentId, kursId);
            var zavrseneLekcije = progress.Where(p => p.JeZavrsena).ToList();

            return Ok(zavrseneLekcije);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("zavrsi")]
        public async Task<IActionResult> ZavrsiLekciju(int kursId, int id)
        {
            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var result = await _studentLekcijaProgressRepository.OznaciLekcijuKaoZavrsenu(validationResult.StudentId, kursId, id);
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
            var validationResult = await _studentValidationHelper.ValidateStudent(User, kursId);
            if (!validationResult.isValid)
            {
                return validationResult.ErrorMessage.Contains("Unauthorized") ? Unauthorized(validationResult.ErrorMessage) : Forbid(validationResult.ErrorMessage);
            }

            var result = await _studentLekcijaProgressRepository.OpozoviZavrsenuLekciju(validationResult.StudentId, kursId, id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
