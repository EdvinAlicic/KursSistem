using AutoMapper;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KursSistemDiplomskiRad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository) {
            _studentRepository = studentRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllStudenti")]
        public async Task<IActionResult> GetAllStudenti()
        {
            var studenti = await _studentRepository.GetAllStudentiAsync();
            return Ok(studenti);
        }
    }
}
