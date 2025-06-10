using System.Net.NetworkInformation;

namespace KursSistemDiplomskiRad.Helpers
{
    public class PasswordHelper
    {
        public static bool IsValidPassword(string password)
        {
            if(string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            return true;
        }
    }
}
