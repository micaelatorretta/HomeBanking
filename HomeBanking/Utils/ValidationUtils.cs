using HomeBanking.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace HomeBanking.Utils
{
    public class ValidationUtils
    {
        public static bool IsNameValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$") && name.Length >= 3;
        }

        public static bool IsPasswordValid(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$");
        }

        public static bool IsValidEmail(string email)
        {
            // Verificación básica del formato de correo electrónico usando una expresión regular
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return false;
            }

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

       

    }
}
