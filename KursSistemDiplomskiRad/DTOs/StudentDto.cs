using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }   
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        // Navigacione osobine
        public List<string> Kursevi { get; set; } // Lista naziva kurseva ili ID-jeva
    }

    public class IspisStudenataDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public DateTime? ZadnjaPrijava { get; set; }
    }
}