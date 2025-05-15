namespace KursSistemDiplomskiRad.DTOs
{
    public class StudentKursReadDto
    {
        public int StudentId { get; set; }
        public int KursId { get; set; }
    }

    public class StudentKursCreateDto
    {
        public int StudentId { get; set; }
        public int KursId { get; set; }
    }
}
