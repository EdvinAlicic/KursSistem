using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IStudentLekcijaProgressRepository
    {
        Task<bool> OznaciLekcijuKaoZavrsenu(int studentId, int kursId, int lekcijaId);
        Task<KursProgressDto> GetKursProgressZaStudenta(int studentId, int kursId);
        Task<IEnumerable<StudentLekcijaProgressDto>> GetProgressZaLekcije(int studentId, int kursId);
        Task<bool> OpozoviZavrsenuLekciju(int studentId, int kursId, int lekcijaId);
    }
}
