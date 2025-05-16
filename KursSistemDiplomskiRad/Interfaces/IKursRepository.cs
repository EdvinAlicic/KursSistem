using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursRepository
    {
        Task<IEnumerable<KursDto>> GetAllKurseviAsync(); // Vraća kolekciju kurseva
        Task<KursDto> GetKursByIdAsync(int id); // Vraća jedan kurs prema ID-u
        Task<KursDto> AddKursAsync(KursCreateDto kurs); // Dodaje novi kurs
        Task<KursDto> UpdateKursAsync(int id, KursUpdateDto kurs); // Ažurira postojeći kurs
        Task<KursDto> DeleteKursAsync(int id); // Briše kurs prema ID-u
    }
}