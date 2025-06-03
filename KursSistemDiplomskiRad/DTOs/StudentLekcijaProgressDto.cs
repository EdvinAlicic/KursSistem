namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentLekcijaProgressDto
    {
        public int LekcijaId { get; set; }
        public bool JeZavrsena { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
    }

    public class KursProgressDto
    {
        public int KursId { get; set; }
        public int StudentId { get; set; }
        public float ProcenatZavrsenog { get; set; }
    }
}
