using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<IspisStudenataDto>> GetAllStudentiAsync();
        Task<IEnumerable<KursIspisZaStudentaDto>> IspisKursevaZaStudenta(int studentId);
        Task<IEnumerable<StudentOnKursDto>> IspisStudenataNaKursu(int kursId);
        Task<bool> DodajStudentaNaKurs(int studentId, int kursId);
        Task<bool> UkloniStudentaSaKursa(int studentId, int kursId);
    }
}
