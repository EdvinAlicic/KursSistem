using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursRepository
    {
        Task<IEnumerable<KursDto>> GetAllKurseviAsync(); // Vraća kolekciju kurseva
        Task<KursDto> GetKursByIdAsync(int id); // Vraća jedan kurs prema ID-u
        Task<IEnumerable<KursDto>> GetAllNeaktivneKurseve();
        Task<IEnumerable<KursDto>> GetNeaktivneKurseveZaStudenta(int studentId);
        Task<KursDto> AddKursAsync(KursCreateDto kurs); // Dodaje novi kurs
        Task<string> PrijavaNaKurs(int studentId, int kursId);
        Task<string> OdjavaSaKursa(int studentId, int kursId);
        Task<KursDto> UpdateKursAsync(int id, KursUpdateDto kurs); // Ažurira postojeći kurs
        Task<KursDto> DeleteKursAsync(int id); // Briše kurs prema ID-u
    }
}