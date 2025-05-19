namespace KursSistemDiplomskiRad.DTOs
{
    public class KursDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; } // Aktivno, Neaktivno
        public List<StudentOnKursDto> Studenti { get; set; } // Lista imena studenata prijavljenih na kurs
        public List<LekcijaDto> Lekcije { get; set; } // Lista lekcija u kursu
    }

    public class KursUpdateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
    }

    public class KursCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
    }

    public class KursIspisZaStudentaDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string StatusKursa { get; set; }
    }

    public class LekcijaCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string MedijskiSadrzaj { get; set; }
    }
}