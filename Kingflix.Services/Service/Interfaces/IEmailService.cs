using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingflix.Services.Interfaces
{
    public interface IEmailService
    {
        bool SendExpired(string email, string fullName, List<Profile> profile, EmailType type);
        bool SendEmailCustom(string email, EmailTemplate emailTemplate);
        bool SendEmailForgotPassword(string email, string fullName, string resetPasswordLink);
        bool SendOrderPending(Order order);
        bool SendOrderSuccess(Order order);
        bool SendOrderCancel(Order order);
        void AddEmailHistory(EmailTypeHistory type, string email);
        IEnumerable<EmailTemplate> GetEmailTemplateList();
        EmailTemplate GetEmailtemplateById(int? id);
        IEnumerable<EmailHistory> GetEmailHistoryList(Expression<Func<EmailHistory, bool>> predicate = null);
        void CreateEmailTemplate(EmailTemplate email);
        void UpdateEmailTemplate(EmailTemplate email);
        void DeleteEmailTemplate(int id);
    }
}