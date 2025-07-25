﻿using KursSistemDiplomskiRad.DTOs;

namespace KursSistemDiplomskiRad.Interfaces
{
    public interface IKursOcjenaRepository
    {
        Task<KursOcjenaDto> DodajOcjenuAsync(int studentId, int kursId, KursOcjenaCreateDto kursOcjenaDto);
        Task<float?> GetProsjecnaOcjenaAsync(int kursId);
        Task<List<KursOcjenaPrikazDto>> GetOcjeneZaKursAsync(int kursId);
        Task<bool> UpdateOcjenaAsync(int studentId, int kursId, KursOcjenaUpdateDto kursOcjenaUpdateDto);
        Task<bool> DeleteOcjenaAsync(int studentId, int kursId);
    }
}
