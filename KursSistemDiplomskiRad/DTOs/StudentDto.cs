using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentReadDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public List<string> Kursevi { get; set; } // Lista naziva kurseva na koje je student prijavljen
    }

    public class StudentCreateDto
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
    }

    public class StudentUpdateDto
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
    }
}