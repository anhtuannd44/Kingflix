using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Kingflix.Utilities
{
    public class HelperFunction
    {
        public static string ConvertToNonUTF(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string DeleteSpace(string chuoi)
        {
            StringBuilder kq = new StringBuilder();
            chuoi = chuoi.Trim();
            for (int i = 0; i < chuoi.Length; i++)
            {
                kq.Append(chuoi[i]);
                if (chuoi[i] == ' ')
                {
                    while (chuoi[i] == ' ')
                    {
                        i++;
                    }
                    kq.Append(chuoi[i]);
                }
            }
            return kq.ToString();
        }

        public static string CreateUrl(string text)
        {
            for (int i = 32; i < 48; i++)
            {

                text = text.Replace(((char)i).ToString(), " ");

            }
            text = text.Replace(".", "-");

            text = text.Replace(" ", "-");

            text = text.Replace(",", "-");

            text = text.Replace(";", "-");

            text = text.Replace(":", "-");

            text = text.Replace("?", "");

            text = text.Replace("&", "-");


            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");

            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);

            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToLower();

        }

        public static string RandomString(int count)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < count; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public string CreateReferralCode(string email)
        {
            return email.Substring(0, email.IndexOf('@'));
        }
        public static string GetYoutubeVideoId(string link)
        {
            link = link.Remove(0, link.IndexOf('=') + 1);
            var index2 = link.IndexOf('&');
            if (index2 != -1)
                link = link.Remove(index2);
            return link;
        }

        public static string EncodeTo64(string toEncode)
        {
            toEncode = toEncode.Replace(Environment.NewLine, "").Replace(" :", ":").Replace(": ", ":").Replace("{ ", "{").Replace("{  ", "{").Replace(" }", "}").Trim();

            string[] textToRemove = new string[4] { " :", ": ", "{ ", " }" };
            string[] textReplace = new string[4] { ":", ":", "{", "}" };

            for (int i = 0; i <= 3; i++)
            {
                var isDoneString = false;
                while (!isDoneString)
                {
                    if (toEncode.Contains(textToRemove[i]))
                        toEncode = toEncode.Replace(textToRemove[i], textReplace[i]);
                    else
                        isDoneString = true;
                }
            }

            byte[] bytes = Encoding.UTF8.GetBytes(toEncode);
            string encodedString = Convert.ToBase64String(bytes);
            return encodedString.Replace("=", "").Replace("/", "_").Replace("+", "-");
        }
    }
}