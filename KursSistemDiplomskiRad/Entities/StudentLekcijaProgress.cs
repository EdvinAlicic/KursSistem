namespace KursSistemDiplomskiRad.Entities
{
    public class StudentLekcijaProgress
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int KursId { get; set; }
        public int LekcijaId { get; set; }
        public bool JeZavrsena { get; set; }
        public DateTime DatumZavrsetka { get; set; }

        public Student Student { get; set; }
        public Kurs Kurs { get; set; }
        public Lekcije Lekcija { get; set; }
    }
}
