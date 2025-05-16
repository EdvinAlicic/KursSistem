using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface ILekcijeRepository
    {
        Task<IEnumerable<LekcijaDto>> GetAllLekcijeAsync(int kursId);
        Task<LekcijaDto> GetLekcijaByIdAsync(int id, int kursId);
        Task<LekcijaDto> AddLekcijaAsync(LekcijaCreateDto lekcija, int kursId);
        Task<LekcijaDto> UpdateLekcijaAsync(int id, LekcijaDto updatedLekcija, int kursId);
        Task<bool> DeleteLekcijaAsync(int id, int kursId);
    }
}
