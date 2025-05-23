namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentOnKursDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public DateTime DatumPrijave { get; set; }
        public string StatusPrijave { get; set; }
    }
}
