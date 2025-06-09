using KursSistemDiplomskiRad.Data;
using KursSistemDiplomskiRad.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KursSistemDiplomskiRad.Helpers
{
    public class StudentValidationHelper
    {
        private readonly DataContext _dataContext;
        public StudentValidationHelper(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<(bool isValid, string ErrorMessage, int StudentId)> ValidateStudent(ClaimsPrincipal user, int kursId)
        {
            var email = user.GetUserEmail();
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Unauthorized: Email ne postoji", 0);
            }

            var student = await _dataContext.Studenti.FirstOrDefaultAsync(s => s.Email == email);
            if(student == null)
            {
                return (false, "Unauthorized: Student ne postoji", 0);
            }

            var prijavljen = await _dataContext.StudentKurs.AnyAsync(sk => sk.StudentId == student.Id && sk.KursId == kursId);
            if (!prijavljen)
            {
                return (false, "Unauthorized: Niste prijavljeni", 0);
            }

            return (true, string.Empty, student.Id);
        }
    }
}
