namespace KursSistemDiplomskiRad.DTOs
{
    public class KursDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; } // Aktivno, Neaktivno
        public List<string> Studenti { get; set; } // Lista imena studenata prijavljenih na kurs
        public List<LekcijaDto> Lekcije { get; set; } // Lista lekcija u kursu
    }

    public class KursCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
        public List<LekcijaCreateDto> Lekcije { get; set; }
    }

    public class LekcijaCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string MedijskiSadrzaj { get; set; }
    }
}