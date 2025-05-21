namespace KursSistemDiplomskiRad.Entities
{
    public class Kurs
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int StatusKursa { get; set; } // Aktivno, Neaktivno
        // Navigacione osobine
        public ICollection<Student> Studenti { get; set; } = new List<Student>();
        public ICollection<Lekcije> Lekcije { get; set; } = new List<Lekcije>();
        public ICollection<StudentKurs> StudentKursevi { get; set; } = new List<StudentKurs>();
    }
}
