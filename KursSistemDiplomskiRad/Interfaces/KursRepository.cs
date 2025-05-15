using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public class KursRepository : IKursRepository
    {
        public Task<KursCreateDto> CreateKursAsync(KursCreateDto kursCreatDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteKursAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<KursReadDto>> GetAllKurseviAsync()
        {
            throw new NotImplementedException();
        }

        public Task<KursReadDto> GetKursByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<KursUpdateDto> UpdateKursAsync(int id, KursUpdateDto kursUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
