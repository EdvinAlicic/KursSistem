using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Repositories
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
        public async Task<LekcijaDto> AddLekcijaAsync(LekcijaCreateDto lekcijaDto, int kursId)
        {
            var kurs = await _dataContext.Kursevi.FindAsync(kursId);
            if (kurs == null)
                return null;

            string? filePath = null;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if(lekcijaDto.MedijskiSadrzaj != null && lekcijaDto.MedijskiSadrzaj.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(lekcijaDto.MedijskiSadrzaj.FileName);
                filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lekcijaDto.MedijskiSadrzaj.CopyToAsync(stream);
                }
            }

            var lekcijaEntity = new Lekcije
            {
                Naziv = lekcijaDto.Naziv,
                Opis = lekcijaDto.Opis,
                MedijskiSadrzaj = filePath != null ? "/uploads/" + Path.GetFileName(filePath) : null,
                KursId = kursId
            };

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

        public async Task<LekcijaDto> UpdateLekcijaAsync(int id, LekcijaZaUpdateDto lekcijaDto, int kursId)
        {
            var lekcija = await _dataContext.Lekcije.FirstOrDefaultAsync(l => l.Id == id && l.KursId == kursId);
            if (lekcija == null)
                return null;

            if (lekcijaDto.Naziv != null)
                lekcija.Naziv = lekcijaDto.Naziv;

            if (lekcijaDto.Opis != null)
                lekcija.Opis = lekcijaDto.Opis;

            if (lekcijaDto.MedijskiSadrzaj != null && lekcijaDto.MedijskiSadrzaj.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(lekcijaDto.MedijskiSadrzaj.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lekcijaDto.MedijskiSadrzaj.CopyToAsync(stream);
                }

                lekcija.MedijskiSadrzaj = "/uploads/" + fileName;
            }

            await _dataContext.SaveChangesAsync();

            return _mapper.Map<LekcijaDto>(lekcija);
        }
    }
}
