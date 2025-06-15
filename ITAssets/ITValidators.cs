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



    }
}
