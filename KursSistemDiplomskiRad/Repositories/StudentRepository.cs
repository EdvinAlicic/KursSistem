using AutoMapper;
using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;
using KursSistemDiplomskiRad.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Repositories
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
                .AsNoTracking()
                .Where(s => s.Role == "Student")
                .ToListAsync();

            return _mapper.Map<IEnumerable<IspisStudenataDto>>(studenti);
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

            var kursevi = prijave.Select(sk => sk.Kurs).ToList();
            return _mapper.Map<IEnumerable<KursIspisZaStudentaDto>>(kursevi);
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
            var studentiNaKursu = await _dataContext.StudentKurs
                .Include(sk => sk.Student)
                .Where(sk => sk.KursId == kursId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentOnKursDto>>(studentiNaKursu);
        }

        public async Task<bool> UpdateStudentAsync(int studentId, StudentUpdateDto studentUpdateDto)
        {
            var student = await _dataContext.Studenti.FindAsync(studentId);
            if (student == null) return false;

            if (studentUpdateDto.Ime != null) student.Ime = studentUpdateDto.Ime;
            if (studentUpdateDto.Prezime != null) student.Prezime = studentUpdateDto.Prezime;
            if (studentUpdateDto.Telefon != null) student.Telefon = studentUpdateDto.Telefon;
            if (studentUpdateDto.Adresa != null) student.Adresa = studentUpdateDto.Adresa;
            if (studentUpdateDto.Email != null) student.Email = studentUpdateDto.Email;

            if(studentUpdateDto.CurrentPassword != null && studentUpdateDto.NewPassword != null)
            {
                var hasher = new PasswordHasher<Student>();
                var result = hasher.VerifyHashedPassword(student, student.Password, studentUpdateDto.CurrentPassword);
                if(result == PasswordVerificationResult.Success)
                {
                    student.Password = hasher.HashPassword(student, studentUpdateDto.NewPassword);
                }
                else
                {
                    return false;
                }
            }

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<IspisStudenataDto> GetStudentById(int studentId)
        {
            var student = await _dataContext.Studenti
                .Include(s => s.StudentKursevi)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if(student == null)
            {
                return null;
            }

            return _mapper.Map<IspisStudenataDto>(student);
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = await _dataContext.Studenti
                .Include(s => s.StudentKursevi)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if(student == null)
            {
                return false;
            }

            _dataContext.StudentKurs.RemoveRange(student.StudentKursevi);

            var tokens = _dataContext.RefreshTokens.Where(rt => rt.StudentId == studentId);
            _dataContext.RefreshTokens.RemoveRange(tokens);

            _dataContext.Studenti.Remove(student);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}
