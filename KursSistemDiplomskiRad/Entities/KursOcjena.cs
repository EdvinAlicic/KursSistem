namespace KursSistemDiplomskiRad.Entities
{
    public class KursOcjena
    {
        public int Id { get; set; }
        public int KursId { get; set; }
        public Kurs Kurs { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int Ocjena { get; set; }
        public string Komentar { get; set; }
        public DateTime Datum { get; set; }
    }
}
