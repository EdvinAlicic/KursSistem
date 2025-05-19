using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public StudentRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<IspisStudenataDto>> GetAllStudentiAsync()
        {
            var studenti = await _dataContext.Studenti.ToListAsync();
            return studenti.Select(s => new IspisStudenataDto
            {
                Id = s.Id,
                Ime = s.Ime,
                Prezime = s.Prezime,
                Email = s.Email,
                Telefon = s.Telefon,
                Adresa = s.Adresa
            }).ToList();
        }
    }
}
