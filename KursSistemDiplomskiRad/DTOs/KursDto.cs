namespace KursSistemDiplomskiRad.DTOs
{
    public class KursDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }
        public int BrojPolaznika { get; set; }
        public List<LekcijeDto> Lekcije { get; set; } = new List<LekcijeDto>();
        public List<StudentDto> Polaznici { get; set; } = new List<StudentDto>();
    }
}
