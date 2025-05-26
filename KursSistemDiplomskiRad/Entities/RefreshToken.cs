namespace KursSistemDiplomskiRad.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
