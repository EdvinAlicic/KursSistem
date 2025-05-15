using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public int BrojIndeksa { get; set; }
        // Navigacione osobine
        public ICollection<StudentKurs> StudentKursevi { get; set; } = new List<StudentKurs>();

    }
}
