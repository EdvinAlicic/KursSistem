using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class StudentLekcijaRepository : IStudentLekcijaProgressRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public StudentLekcijaRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<KursProgressDto> GetKursProgressZaStudenta(int studentId, int kursId)
        {
            var ukupnoLekcija = await _dataContext.Lekcije.CountAsync(l => l.KursId == kursId);
            if(ukupnoLekcija == 0)
            {
                return new KursProgressDto
                {
                    KursId = kursId,
                    StudentId = studentId,
                    ProcenatZavrsenog = 0
                };
            }

            var zavrseneLekcije = await _dataContext.StudentLekcijaProgress
                .CountAsync(p => p.StudentId == studentId && p.KursId == kursId && p.JeZavrsena);

            return new KursProgressDto
            {
                KursId = kursId,
                StudentId = studentId,
                ProcenatZavrsenog = (float)zavrseneLekcije / ukupnoLekcija * 100
            };
        }

        public async Task<IEnumerable<StudentLekcijaProgressDto>> GetProgressZaLekcije(int studentId, int kursId)
        {
            var progress = await _dataContext.StudentLekcijaProgress
                .Where(p => p.StudentId == studentId && p.KursId == kursId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentLekcijaProgressDto>>(progress);
        }

        public async Task<bool> OpozoviZavrsenuLekciju(int studentId, int kursId, int lekcijaId)
        {
            var progress = await _dataContext.StudentLekcijaProgress
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.KursId == kursId && p.LekcijaId == lekcijaId);

            if(progress == null || !progress.JeZavrsena)
            {
                return false;
            }

            progress.JeZavrsena = false;
            progress.DatumZavrsetka = default;
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OznaciLekcijuKaoZavrsenu(int studentId, int kursId, int lekcijaId)
        {
            var progress = await _dataContext.StudentLekcijaProgress
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.KursId == kursId && p.LekcijaId == lekcijaId);

            if(progress == null)
            {
                progress = new StudentLekcijaProgress
                {
                    StudentId = studentId,
                    KursId = kursId,
                    LekcijaId = lekcijaId,
                    JeZavrsena = true,
                    DatumZavrsetka = DateTime.Now
                };
                await _dataContext.StudentLekcijaProgress.AddAsync(progress);
            }
            else
            {
                progress.JeZavrsena = true;
                progress.DatumZavrsetka = DateTime.Now;
            }

            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}
