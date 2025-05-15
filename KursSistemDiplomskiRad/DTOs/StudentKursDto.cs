using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentKursDto
    {
        public int StudentId { get; set; }
        public int KursId { get; set; }
        public DateTime DatumPrijave { get; set; }
        public string StatusPrijave { get; set; }
        // Opcionalno: Dodajte imena radi lakšeg prikaza
        public string StudentIme { get; set; }
        public string KursNaziv { get; set; }
    }
}
