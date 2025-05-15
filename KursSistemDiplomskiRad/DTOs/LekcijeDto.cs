using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class LekcijaReadDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int KursId { get; set; } // ID kursa kojem lekcija pripada
    }

    public class LekcijaCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public byte[] MedijskiSadrzaj { get; set; }
        public int KursId { get; set; }
    }
}