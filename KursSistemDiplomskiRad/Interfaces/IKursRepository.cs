using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursRepository
    {
        Task<IEnumerable<KursBasicDto>> GetAllKurseviAsync();
        Task<KursBasicDto> GetKursByIdAsync(int id);
        Task<IEnumerable<KursBasicDto>> GetAllNeaktivneKurseve();
        Task<IEnumerable<KursBasicDto>> GetNeaktivneKurseveZaStudenta(int studentId);
        Task<IEnumerable<KursBasicDto>> SearchKurseviAsync(string searchTerm);
        Task<KursDto> AddKursAsync(KursCreateDto kurs);
        Task<bool> PrijavaNaKurs(int studentId, int kursId);
        Task<bool> OdjavaSaKursa(int studentId, int kursId);
        Task<KursDto> UpdateKursAsync(int id, KursUpdateDto kurs);
        Task<KursDto> DeleteKursAsync(int id);
    }
}