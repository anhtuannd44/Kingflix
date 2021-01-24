namespace Kingflix.Utilities
{
    public class MembershipHelper
    {
        public static string GetMemberShip(string email, int referralInt)
        {
            return email.Substring(0, email.IndexOf('@')).Replace(".", string.Empty).ToUpper() + referralInt;
        }
    }
}