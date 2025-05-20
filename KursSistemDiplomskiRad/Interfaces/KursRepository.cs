using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class KursRepository : IKursRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        public KursRepository(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }
        public async Task<IEnumerable<KursDto>> GetAllKurseviAsync()
        {
            var kursevi = await _dataContext.Kursevi
                //.Include(k => k.Lekcije)
                //.Include(k => k.StudentKursevi)
                    //.ThenInclude(sk => sk.Student)
                .ToListAsync();
            return _mapper.Map<IEnumerable<KursDto>>(kursevi);
        }

        public async Task<KursDto> GetKursByIdAsync(int id)
        {
            var kursevi = await _dataContext.Kursevi.FindAsync(id);
            if(kursevi == null)
            {
                return null;
            }

            return _mapper.Map<KursDto>(kursevi);
        }

        public async Task<KursDto> AddKursAsync(KursCreateDto kurs)
        {
            try
            {
                if(kurs == null)
                {
                    throw new ArgumentNullException(nameof(kurs));
                }
                var kursEntity = _mapper.Map<Entities.Kurs>(kurs);
                var createdKurs = await _dataContext.Kursevi.AddAsync(kursEntity);
                await _dataContext.SaveChangesAsync();
                var createdKursDto = _mapper.Map<KursDto>(createdKurs.Entity);
                return createdKursDto;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<KursDto> UpdateKursAsync(int id, KursUpdateDto updatedKurs)
        {
            var kursEntity = await _dataContext.Kursevi.FindAsync(id);
            if(kursEntity == null)
            {
                return null;
            }
            _mapper.Map(updatedKurs, kursEntity);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<KursDto>(kursEntity);
        }

        public async Task<KursDto> DeleteKursAsync(int id)
        {
            try
            {
                var kursEntity = await _dataContext.Kursevi.FindAsync(id);
                if (kursEntity == null)
                {
                    return null;
                }
                var deletedKurs = _mapper.Map<KursDto>(kursEntity);
                _dataContext.Kursevi.Remove(kursEntity);
                await _dataContext.SaveChangesAsync();
                return deletedKurs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
