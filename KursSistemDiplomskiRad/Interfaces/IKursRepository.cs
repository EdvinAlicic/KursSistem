using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursRepository
    {
        Task<IEnumerable<KursBasicDto>> GetAllKurseviAsync(); // Vraća kolekciju kurseva
        Task<KursBasicDto> GetKursByIdAsync(int id); // Vraća jedan kurs prema ID-u
        Task<IEnumerable<KursBasicDto>> GetAllNeaktivneKurseve();
        Task<IEnumerable<KursBasicDto>> GetNeaktivneKurseveZaStudenta(int studentId);
        Task<KursDto> AddKursAsync(KursCreateDto kurs); // Dodaje novi kurs
        Task<bool> PrijavaNaKurs(int studentId, int kursId);
        Task<bool> OdjavaSaKursa(int studentId, int kursId);
        Task<KursDto> UpdateKursAsync(int id, KursUpdateDto kurs); // Ažurira postojeći kurs
        Task<KursDto> DeleteKursAsync(int id); // Briše kurs prema ID-u
    }
}