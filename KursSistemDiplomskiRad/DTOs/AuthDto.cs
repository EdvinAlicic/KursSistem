namespace KursSistemDiplomskiRad.DTOs
{
    public class RegisterDto
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
