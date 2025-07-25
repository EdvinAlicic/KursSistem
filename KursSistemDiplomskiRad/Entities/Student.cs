﻿namespace KursSistemDiplomskiRad.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public string Role { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public DateTime? ZadnjaPrijava { get; set; }
        // Navigacione osobine
        public ICollection<StudentKurs> StudentKursevi { get; set; } = new List<StudentKurs>();
    }
}
