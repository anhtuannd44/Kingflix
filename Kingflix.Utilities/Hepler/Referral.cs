using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Kingflix.Utilities
{
    public class ReferralHelper
    {
        public static string GenerateReferralCode(string email, int referralInt)
        {
            return email.Substring(0, email.IndexOf('@')).Replace(".", string.Empty).ToUpper() + referralInt;
        }
    }
}