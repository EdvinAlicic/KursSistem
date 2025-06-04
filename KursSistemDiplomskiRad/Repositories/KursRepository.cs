using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Repositories
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
        public async Task<IEnumerable<KursBasicDto>> GetAllKurseviAsync()
        {
            var kursevi = await _dataContext.Kursevi
                .Where(k => k.StatusKursa == 1)
                .ToListAsync();
            return _mapper.Map<IEnumerable<KursBasicDto>>(kursevi);
        }

        public async Task<KursBasicDto> GetKursByIdAsync(int id)
        {
            var kursevi = await _dataContext.Kursevi.FindAsync(id);
            if(kursevi == null)
            {
                return null;
            }

            return _mapper.Map<KursBasicDto>(kursevi);
        }

        public async Task<KursDto> AddKursAsync(KursCreateDto kursDto)
        {
            if (kursDto == null)
            {
                return null;
            }

            var kursEntity = _mapper.Map<Kurs>(kursDto);
            var createdKurs = await _dataContext.Kursevi.AddAsync(kursEntity);
            await _dataContext.SaveChangesAsync();
            var createdKursDto = _mapper.Map<KursDto>(createdKurs.Entity);
            return createdKursDto;
        }

        public async Task<KursDto> UpdateKursAsync(int id, KursUpdateDto updatedKurs)
        {
            var kursEntity = await _dataContext.Kursevi.FindAsync(id);

            if(kursEntity == null)
            {
                return null;
            }

            if(updatedKurs.Naziv != null)
            {
                kursEntity.Naziv = updatedKurs.Naziv;
            }

            if(updatedKurs.Opis != null)
            {
                kursEntity.Opis = updatedKurs.Opis;
            }

            if (updatedKurs.StatusKursa.HasValue)
            {
                kursEntity.StatusKursa = updatedKurs.StatusKursa.Value;
            }

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

        public async Task<bool> PrijavaNaKurs(int studentId, int kursId)
        {
            var student = await _dataContext.Studenti.FindAsync(studentId);
            var kurs = await _dataContext.Kursevi.FindAsync(kursId);

            if(student == null || kurs == null || kurs.StatusKursa != 1)
            {
                return false;
            }

            var postoji = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == studentId && sk.KursId == kursId);

            if (postoji)
            {
                return false;
            }

            var prijava = new StudentKurs
            {
                StudentId = studentId,
                KursId = kursId,
                DatumPrijave = DateTime.Now,
                StatusPrijave = "Aktivan"
            };

            await _dataContext.StudentKurs.AddAsync(prijava);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OdjavaSaKursa(int studentId, int kursId)
        {
            var prijava = await _dataContext.StudentKurs
                .FirstOrDefaultAsync(sk => sk.StudentId == studentId && sk.KursId == kursId);

            if (prijava == null)
            {
                return false;
            }

            _dataContext.StudentKurs.Remove(prijava);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<KursBasicDto>> GetAllNeaktivneKurseve()
        {
            var neaktivniKursevi = await _dataContext.Kursevi.Where(k => k.StatusKursa == 0).ToListAsync();
            return _mapper.Map<IEnumerable<KursBasicDto>>(neaktivniKursevi);
        }

        public async Task<IEnumerable<KursBasicDto>> GetNeaktivneKurseveZaStudenta(int studentId)
        {
            var kursId = await _dataContext.StudentKurs
                .Where(sk => sk.StudentId == studentId)
                .Select(sk => sk.KursId)
                .ToListAsync();

            var neaktivniKursevi = await _dataContext.Kursevi
                .Where(k => k.StatusKursa == 0 && kursId.Contains(k.Id))
                .ToListAsync();

            return _mapper.Map<IEnumerable<KursBasicDto>>(neaktivniKursevi);
        }
    }
}
