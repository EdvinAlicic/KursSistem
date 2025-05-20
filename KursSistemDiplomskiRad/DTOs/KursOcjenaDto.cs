namespace KursSistemDiplomskiRad.DTOs
{
    public class KursOcjenaDto
    {
        public int Ocjena { get; set; }
        public string Komentar { get; set; }
    }

    public class KursOcjenaPrikazDto
    {
        public int Ocjena { get; set; }
        public string Komentar { get; set; }
        public string ImeStudenta { get; set; }
        public string PrezimeStudenta { get; set; }
    }
}
