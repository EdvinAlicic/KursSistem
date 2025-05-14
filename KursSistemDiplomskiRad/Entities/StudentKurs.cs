namespace KursSistemDiplomskiRad.Entities
{
    public class StudentKurs
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int KursId { get; set; }
        public Kurs Kurs { get; set; }
    }
}
