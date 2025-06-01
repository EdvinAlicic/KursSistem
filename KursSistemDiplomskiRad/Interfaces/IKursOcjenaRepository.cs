using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursOcjenaRepository
    {
        Task<bool> DodajOcjenuAsync(int studentId, int kursId, KursOcjenaDto kursOcjenaDto);
        Task<float?> GetProsjecnaOcjenaAsync(int kursId);
        Task<List<KursOcjenaPrikazDto>> GetOcjeneZaKursAsync(int kursId);
        Task<bool> UpdateOcjenaAsync(int studentId, int kursId, KursOcjenaUpdateDto kursOcjenaUpdateDto);
        Task<bool> DeleteOcjenaAsync(int studentId, int kursId);
    }
}
