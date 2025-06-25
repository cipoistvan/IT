using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITAssets
{
    public sealed class ITValidators
    {
        public static bool ValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            

            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }


        public static bool ValidatePurchaseYear(DateTime? date)
        {
            int currentYear = DateTime.Now.Year;
            var year = date?.Year;
            return year >= 2000 && year <= currentYear;
        }

        public static bool ValidateQty(int qty)
        {
            if (qty < 1 || qty > 100)
                return false;
            return true;
        }





    }
}
