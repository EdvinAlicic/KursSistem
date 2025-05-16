using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class LekcijeRepository : ILekcijeRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public LekcijeRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<LekcijaDto> AddLekcijaAsync(LekcijaCreateDto lekcija, int kursId)
        {
            var kurs = await _dataContext.Kursevi.FindAsync(kursId);
            if(kurs == null)
            {
                return null;
            }
            var lekcijaEntity = _mapper.Map<Lekcije>(lekcija);
            lekcijaEntity.KursId = kursId;
            await _dataContext.Lekcije.AddAsync(lekcijaEntity);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<LekcijaDto>(lekcijaEntity);
        }

        public async Task<bool> DeleteLekcijaAsync(int id, int kursId)
        {
            var lekcija = _dataContext.Lekcije.FirstOrDefault(l => l.Id == id && l.KursId == kursId);
            if (lekcija == null)
            {
                return false;
            }
            _dataContext.Lekcije.Remove(lekcija);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LekcijaDto>> GetAllLekcijeAsync(int kursId)
        {
            var lekcije = await _dataContext.Lekcije.Where(l => l.KursId == kursId).ToListAsync();
            return _mapper.Map<IEnumerable<LekcijaDto>>(lekcije);
        }

        public async Task<LekcijaDto> GetLekcijaByIdAsync(int id, int kursId)
        {
            var lekcija = await _dataContext.Lekcije
                .FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if(lekcija == null)
            {
                return null;
            }
            return _mapper.Map<LekcijaDto>(lekcija);
        }

        public async Task<LekcijaDto> UpdateLekcijaAsync(int id, LekcijaDto updatedLekcija, int kursId)
        {
            var lekcija = await _dataContext.Lekcije.FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if(lekcija == null)
            {
                return null;
            }
            lekcija.Naziv = updatedLekcija.Naziv;
            lekcija.Opis = updatedLekcija.Opis;
            lekcija.MedijskiSadrzaj = updatedLekcija.MedijskiSadrzaj;
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<LekcijaDto>(lekcija);
        }
    }
}
