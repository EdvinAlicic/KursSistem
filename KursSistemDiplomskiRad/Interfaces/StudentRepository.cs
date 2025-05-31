using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using Microsoft.AspNetCore.Mvc;
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
            var studenti = await _dataContext.Studenti
                .Where(s => s.Role == "Student")
                .ToListAsync();

            return studenti.Select(s => new IspisStudenataDto
            {
                Id = s.Id,
                Ime = s.Ime,
                Prezime = s.Prezime,
                Email = s.Email,
                Telefon = s.Telefon,
                Adresa = s.Adresa,
                DatumRegistracije = s.DatumRegistracije,
                ZadnjaPrijava = s.ZadnjaPrijava
            }).ToList();
        }

        public async Task<IEnumerable<KursIspisZaStudentaDto>> IspisKursevaZaStudenta(int studentId)
        {
            var prijave = await _dataContext.StudentKurs
                .Include(sk => sk.Kurs)
                .Where(sk => sk.StudentId == studentId)
                .ToListAsync();

            if (prijave == null || prijave.Count == 0)
            {
                return null;
            }

            var kursevi = prijave.Select(sk => new KursIspisZaStudentaDto
            {
                Id = sk.Kurs.Id,
                Naziv = sk.Kurs.Naziv,
                Opis = sk.Kurs.Opis,
                StatusKursa = sk.Kurs.StatusKursa
            }).ToList();

            return kursevi;
        }

        public async Task<bool> DodajStudentaNaKurs(int studentId, int kursId)
        {
            var student = await _dataContext.Studenti.FindAsync(studentId);
            var kurs = await _dataContext.Kursevi.FindAsync(kursId);
            if(student == null || kurs == null)
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

        public async Task<bool> UkloniStudentaSaKursa(int studentId, int kursId)
        {
            var prijava = await _dataContext.StudentKurs
                .FirstOrDefaultAsync(sk => sk.StudentId == studentId && sk.KursId == kursId);
            if(prijava == null)
            {
                return false;
            }

            _dataContext.StudentKurs.Remove(prijava);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentOnKursDto>> IspisStudenataNaKursu(int kursId)
        {
            var studentiNaKursu = _dataContext.StudentKurs
                .Include(sk => sk.Student)
                .Where(sk => sk.KursId == kursId)
                .Select(sk => new StudentOnKursDto
                {
                    Id = sk.Student.Id,
                    Ime = sk.Student.Ime,
                    Prezime = sk.Student.Prezime,
                    Email = sk.Student.Email,
                    DatumPrijave = sk.DatumPrijave,
                    StatusPrijave = sk.StatusPrijave
                });

            return await studentiNaKursu.ToListAsync();
        }
    }
}
