namespace KursSistemDiplomskiRad.DTOs
{
    public class KursReadDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; } // Aktivno, Neaktivno
        public List<string> Studenti { get; set; } // Lista imena studenata prijavljenih na kurs
        public List<LekcijaReadDto> Lekcije { get; set; } // Lista lekcija u kursu
    }

    public class KursCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
    }

    public class KursUpdateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
    }
}