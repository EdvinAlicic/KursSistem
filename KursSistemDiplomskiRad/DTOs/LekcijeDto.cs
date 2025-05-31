using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class LekcijaDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string MedijskiSadrzaj { get; set; }
        public int KursId { get; set; } // FK
    }

    public class LekcijaZaUpdateDto
    {
        public string? Naziv { get; set; }
        public string? Opis { get; set; }
        public IFormFile? MedijskiSadrzaj { get; set; }
    }
}