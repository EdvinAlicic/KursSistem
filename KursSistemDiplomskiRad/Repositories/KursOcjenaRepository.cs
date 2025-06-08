using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace KursSistemDiplomskiRad.Repositories
{
    public class KursOcjenaRepository : IKursOcjenaRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public KursOcjenaRepository(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }
        public async Task<KursOcjenaDto> DodajOcjenuAsync(int studentId, int kursId, KursOcjenaCreateDto kursOcjenaCreateDto)
        {
            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == studentId && sk.KursId == kursId);

            if (!prijavljen)
            {
                return null;
            }

            var postoji = await _dataContext.KursOcjene
                .AnyAsync(o => o.StudentId == studentId && o.KursId == kursId);

            if (postoji)
            {
                return null;
            }

            var ocjenaEntity = _mapper.Map<KursOcjena>(kursOcjenaCreateDto);
            ocjenaEntity.StudentId = studentId;
            ocjenaEntity.KursId = kursId;
            ocjenaEntity.Datum = DateTime.Now;

            var createdOcjena = await _dataContext.KursOcjene.AddAsync(ocjenaEntity);
            await _dataContext.SaveChangesAsync();
            var createdOcjenaDto = _mapper.Map<KursOcjenaDto>(createdOcjena.Entity);
            return createdOcjenaDto;
        }

        public async Task<float?> GetProsjecnaOcjenaAsync(int kursId)
        {
            return await _dataContext.KursOcjene
                .Where(o => o.KursId == kursId)
                .Select(o => (float?)o.Ocjena)
                .AverageAsync();
        }

        public async Task<List<KursOcjenaPrikazDto>> GetOcjeneZaKursAsync(int kursId)
        {
            var ocjene = await _dataContext.KursOcjene
                .Include(o => o.Student)
                .Where(o => o.KursId == kursId)
                .ToListAsync();

            return _mapper.Map<List<KursOcjenaPrikazDto>>(ocjene);
        }

        public async Task<bool> UpdateOcjenaAsync(int studentId, int kursId, KursOcjenaUpdateDto kursOcjenaUpdateDto)
        {
            var ocjena = await _dataContext.KursOcjene
                .FirstOrDefaultAsync(o => o.StudentId == studentId && o.KursId == kursId);

            if (ocjena == null)
            {
                return false;
            }

            if (kursOcjenaUpdateDto.Ocjena.HasValue)
            {
                ocjena.Ocjena = kursOcjenaUpdateDto.Ocjena.Value;
            }

            if(kursOcjenaUpdateDto.Komentar != null)
            {
                ocjena.Komentar = kursOcjenaUpdateDto.Komentar;
            }

            ocjena.Datum = DateTime.Now;

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOcjenaAsync(int studentId, int kursId)
        {
            var ocjena = await _dataContext.KursOcjene
                .FirstOrDefaultAsync(o => o.StudentId == studentId && o.KursId == kursId);

            if(ocjena == null)
            {
                return false;
            }

            _dataContext.KursOcjene.Remove(ocjena);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}