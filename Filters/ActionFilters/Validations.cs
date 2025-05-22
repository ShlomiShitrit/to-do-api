using System.Text.RegularExpressions;

namespace Backend.Filters.ActionFilters
{
    static public class Validations
    {
        static public bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            return Regex.IsMatch(email, emailPattern);
        }

        static public string IsValidPassword(string password)
        {
            if (password.Length < 6)
                return "Password must contains at least 6 characters";

            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter";

            if (!password.Any(char.IsLower))
                return "Password must contain at least one lowercase letter";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one digit";

            char[] specialCharacters = "!@#$%^&*()_+{}[];:'\"<>,./?".ToCharArray();

            if (!password.Any(specialCharacters.Contains))
                return "Password must contain at least one special character";

            return "ok";
        }
    }
}