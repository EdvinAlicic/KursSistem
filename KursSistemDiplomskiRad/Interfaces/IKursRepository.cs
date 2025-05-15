using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursRepository
    {
        Task<List<KursReadDto>> GetAllKurseviAsync();
        Task<KursReadDto> GetKursByIdAsync(int id);
        Task<KursCreateDto> CreateKursAsync(KursCreateDto kursCreatDto);
        Task<KursUpdateDto> UpdateKursAsync(int id, KursUpdateDto kursUpdateDto);
        Task DeleteKursAsync(int id);
    }
}
