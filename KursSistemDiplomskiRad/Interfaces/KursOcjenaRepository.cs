using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class KursOcjenaRepository : IKursOcjenaRepository
    {
        private readonly DataContext _dataContext;
        public KursOcjenaRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> DodajOcjenuAsync(int studentId, int kursId, KursOcjenaDto kursOcjenaDto)
        {
            var prijavljen = await _dataContext.StudentKurs
                .AnyAsync(sk => sk.StudentId == studentId && sk.KursId == kursId);

            if (!prijavljen)
            {
                return false;
            }

            var postoji = await _dataContext.KursOcjene
                .AnyAsync(o => o.StudentId == studentId && o.KursId == kursId);

            if (postoji)
            {
                return false;
            }

            var ocjena = new KursOcjena
            {
                StudentId = studentId,
                KursId = kursId,
                Ocjena = kursOcjenaDto.Ocjena,
                Komentar = kursOcjenaDto.Komentar,
                Datum = DateTime.Now
            };

            await _dataContext.KursOcjene.AddAsync(ocjena);
            await _dataContext.SaveChangesAsync();
            return true;
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
            return await _dataContext.KursOcjene.Where(o => o.KursId == kursId)
                .Include(o => o.Student)
                .Select(o => new KursOcjenaPrikazDto
                {
                    Ocjena = o.Ocjena,
                    Komentar = o.Komentar,
                    ImeStudenta = o.Student.Ime,
                    PrezimeStudenta = o.Student.Prezime
                }).ToListAsync();
        }
    }
}
