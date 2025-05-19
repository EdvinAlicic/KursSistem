using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<IspisStudenataDto>> GetAllStudentiAsync();
    }
}
