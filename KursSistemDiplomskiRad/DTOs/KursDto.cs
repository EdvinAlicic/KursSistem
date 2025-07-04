﻿namespace KursSistemDiplomskiRad.DTOs
{
    public class KursDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int StatusKursa { get; set; } // Aktivno, Neaktivno
        public float? ProsjecnaOcjena { get; set; }
    }

    public class KursUpdateDto
    {
        public string? Naziv { get; set; }
        public string? Opis { get; set; }
        public int? StatusKursa { get; set; }
    }

    public class KursCreateDto
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int StatusKursa { get; set; }
    }

    public class KursIspisZaStudentaDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int StatusKursa { get; set; }
    }

    public class LekcijaCreateDto
    {
        public string Naziv { get; set; }
        public string? Opis { get; set; }

        public IFormFile? MedijskiSadrzaj { get; set; }
    }

    public class KursBasicDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public float? ProsjecnaOcjena { get; set; }
        public int StatusKursa { get; set; }
    }
}