using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System.Collections.Generic;

namespace Kingflix.Services.Interfaces
{
    public interface IEmailService
    {
        EmailHistory SendExpired(string email, string fullName, List<Profile> profile, EmailType type);
        EmailHistory SendEmailCustom(string email, EmailTemplate emailTemplate);
        EmailHistory SendEmailForgotPassword(string email, string fullName, string resetPasswordLink);
        EmailHistory SendOrder(Order order, List<Profile> profiles = null);
    }
}