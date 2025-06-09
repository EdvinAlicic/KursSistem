namespace KursSistemDiplomskiRad.Entities
{
    public class Lekcije
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string? Opis { get; set; }
        public string? MedijskiSadrzaj { get; set; }
        public int KursId { get; set; } // FK
        // Navigacione osobine
        public Kurs Kurs { get; set; } // FK
    }
}
