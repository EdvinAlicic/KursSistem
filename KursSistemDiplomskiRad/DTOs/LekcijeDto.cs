using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class LekcijeDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public byte[] MedijskiSadrzaj { get; set; }
        public int KursId { get; set; } // FK
        // Navigacione osobine
        public Kurs Kurs { get; set; } // FK
    }
}
