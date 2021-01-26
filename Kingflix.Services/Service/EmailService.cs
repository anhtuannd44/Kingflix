using System;
using System.IO;
using System.Web.Hosting;
using System.Linq;
using System.Collections.Generic;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using System.Linq.Expressions;
using Kingflix.Services.Interfaces;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace Kingflix.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        public EmailService(
            IUnitOfWork unitOfWork,
            IRepository<EmailHistory> emailHistoryRepository,
            IRepository<EmailTemplate> emailTemplateRepository
            )
        {
            _unitOfWork = unitOfWork;
            _emailHistoryRepository = emailHistoryRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }
        public bool SendExpired(string email, string fullName, List<Profile> profile, EmailType type)
        {
            try
            {
                var account = "";
                foreach (var item in profile)
                {
                    account += "<tr>" +
                                    "<td>" + item.Products.ProductId + "</td>" +
                                    "<td>" + item.Name + "</td>" +
                                    "<td style='color: red;'>" + item.DateEnd.ToString("dd/MM/yyyy") + "</td>" +
                               "</tr>";
                }
                GMailer mailer = new GMailer
                {
                    ToEmail = email
                };
                string template = "";
                if (type == EmailType.PreExpired)
                {
                    template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/PreExpired.html"));
                }
                else if (type == EmailType.Exprired)
                {
                    template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/Expired.html"));
                }
                template = template.Replace("{FullName}", fullName ?? email)
                                    .Replace("{Email}", email)
                                    .Replace("{Account}", account);

                mailer.Subject = "Thông Báo Gia Hạn - Kingflix";
                mailer.Body = template;
                mailer.IsHtml = true;
                mailer.Send();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendEmailCustom(string email, EmailTemplate emailTemplate)
        {
            try
            {
                GMailer mailer = new GMailer
                {
                    ToEmail = email
                };
                mailer.Subject = emailTemplate.Title;
                mailer.Body = emailTemplate.Content;
                mailer.IsHtml = true;
                mailer.Send();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendEmailForgotPassword(string email, string fullName, string resetPasswordLink)
        {
            try
            {
                GMailer mailer = new GMailer
                {
                    ToEmail = email
                };
                var template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/ForgotPassword.html"));
                template = template.Replace("{FullName}", fullName ?? email)
                                    .Replace("{ResetPasswordLink}", resetPasswordLink);
                mailer.Subject = "Yêu cầu tạo lại mật khẩu Kingflix.vn";
                mailer.Body = template;
                mailer.IsHtml = true;
                mailer.Send();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendOrderPending(Order order)
        {
            try
            {
                GMailer mailer = new GMailer
                {
                    ToEmail = order.UserInformation.Email
                };

                string template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/OrderPending.html"));

                template = template.Replace("{LinkOrder}", "https://kingflix.vn/Order/Details?orderId=" + order.OrderId)
                                    .Replace("{OrderId}", order.OrderId)
                                    .Replace("{FullName}", order.UserInformation.FullName)
                                    .Replace("{Email}", order.UserInformation.Email)
                                    .Replace("{PhoneNumber}", order.UserInformation.PhoneNumber)
                                    .Replace("{Status}", order.Status.GetEnumDisplayName())
                                    .Replace("{DateCreate}", order.DateCreated.ToString("HH:mm:ss dd/MM/yyyy"));

                var subject = "";
                if (order.Status == OrderStatus.Done)
                {
                    subject = "Kingflix.vn - Hoàn tất đơn hàng #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.WaitingForPay)
                {
                    subject = "Kingflix.vn - Đơn hàng mới #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.Cencelled)
                {
                    subject = "Kingflix.vn - Hủy đơn #" + order.OrderId;
                }

                mailer.Subject = subject;
                mailer.Body = template;
                mailer.IsHtml = true;
                mailer.Send();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendOrderSuccess(Order order)
        {
            try
            {
                var account = "";
                foreach (var item in order.Profiles)
                {
                    account += "<tr>" +
                                    "<td>" + item.Products.ProductId + "</td>" +
                                    "<td>" + item.Products.Password + "</td>" +
                                    "<td>" + item.Name + "</td>" +
                                    "<td>" + item.Pincode + "</td>" +
                                    "<td>" + item.DateEnd.ToString("dd/MM/yyyy") + "</td>" +
                               "</tr>";
                }
                GMailer mailer = new GMailer
                {
                    ToEmail = order.UserInformation.Email
                };


                string template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/OrderSuccess.html"));

                template = template.Replace("{LinkOrder}", "https://kingflix.vn/Order/Details?orderId=" + order.OrderId)
                                    .Replace("{OrderId}", order.OrderId)
                                    .Replace("{FullName}", order.UserInformation.FullName)
                                    .Replace("{Email}", order.UserInformation.Email)
                                    .Replace("{PhoneNumber}", order.UserInformation.PhoneNumber)
                                    .Replace("{Status}", order.Status.GetEnumDisplayName())
                                    .Replace("{DateCreate}", order.DateCreated.ToString("HH:mm:ss dd/MM/yyyy"))
                                    .Replace("{DateConfirm}", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"))
                                    .Replace("{Account}", account);

                var subject = "";
                if (order.Status == OrderStatus.Done)
                {
                    subject = "Kingflix.vn - Hoàn tất đơn hàng #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.WaitingForPay)
                {
                    subject = "Kingflix.vn - Đơn hàng mới #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.Cencelled)
                {
                    subject = "Kingflix.vn - Hủy đơn #" + order.OrderId;
                }

                mailer.Subject = subject;
                mailer.Body = template;
                mailer.IsHtml = true;
                mailer.Send();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendOrderCancel(Order order)
        {
            try
            {
                GMailer mailer = new GMailer
                {
                    ToEmail = order.UserInformation.Email
                };

                string template = File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplate/OrderCancel.html"));

                template = template.Replace("{LinkOrder}", "https://kingflix.vn/Order/Details?orderId=" + order.OrderId)
                                    .Replace("{OrderId}", order.OrderId)
                                    .Replace("{FullName}", order.UserInformation.FullName)
                                    .Replace("{Email}", order.UserInformation.Email)
                                    .Replace("{PhoneNumber}", order.UserInformation.PhoneNumber)
                                    .Replace("{Status}", order.Status.GetEnumDisplayName())
                                    .Replace("{DateConfirm}", order.DateConfirm.Value.ToString("HH:mm:ss dd/MM/yyyy"))
                                    .Replace("{DateCreate}", order.DateCreated.ToString("HH:mm:ss dd/MM/yyyy"));

                var subject = "";
                if (order.Status == OrderStatus.Done)
                {
                    subject = "Kingflix.vn - Hoàn tất đơn hàng #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.WaitingForPay)
                {
                    subject = "Kingflix.vn - Đơn hàng mới #" + order.OrderId;
                }
                else if (order.Status == OrderStatus.Cencelled)
                {
                    subject = "Kingflix.vn - Hủy đơn #" + order.OrderId;
                }

                mailer.Subject = subject;
                mailer.Body = template;
                mailer.IsHtml = true;
                mailer.Send();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendEmailCustom(string email, string subject, string body)
        {
            try
            {
                GMailer mailer = new GMailer
                {
                    ToEmail = email
                };

                mailer.Subject = subject;
                mailer.Body = body;
                mailer.IsHtml = true;
                mailer.Send();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void AddEmailHistory(EmailTypeHistory type, string email)
        {
            var item = new EmailHistory() { Type = type, Email = email, DateSend = DateTime.Now };
            _emailHistoryRepository.Update(item);
        }
        public IEnumerable<EmailTemplate> GetEmailTemplateList()
        {
            return _emailTemplateRepository.GetAll();
        }
        public EmailTemplate GetEmailtemplateById(int? id)
        {
            return _emailTemplateRepository.GetById(id);
        }
        public void CreateEmailTemplate(EmailTemplate email)
        {
            _emailTemplateRepository.Create(email);
            _unitOfWork.SaveChanges();
        }
        public void UpdateEmailTemplate(EmailTemplate email)
        {
            _emailTemplateRepository.Update(email);
            _unitOfWork.SaveChanges();
        }
        public void DeleteEmailTemplate(int id)
        {
            var email = _emailTemplateRepository.GetById(id);
            _emailTemplateRepository.Delete(email);
            _unitOfWork.SaveChanges();
        }
        public IEnumerable<EmailHistory> GetEmailHistoryList(Expression<Func<EmailHistory, bool>> predicate = null)
        {
            return _emailHistoryRepository.Filter(predicate).AsEnumerable();
        }
    }

    public class EmailJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;
        private readonly IEmailService _emailService;
        public EmailJob(
            IUnitOfWork unitOfWork,
            IRepository<Profile> profileService,
            IRepository<EmailHistory> emailHistoryRepository,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _profileRepository = profileService;
            _emailHistoryRepository = emailHistoryRepository;
            _emailService = emailService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var profiles = _profileRepository.GetAll();
            var listPreExpired = profiles.Where(a => a.DateEnd.AddDays(-7).Date == DateTime.Today.Date && !string.IsNullOrEmpty(a.UserId));
            var listExpired = profiles.Where(a => a.DateEnd.AddDays(1).Date == DateTime.Today.Date && !string.IsNullOrEmpty(a.UserId));
            foreach (var item in listPreExpired)
            {
                try
                {
                    var preExpried = listPreExpired.GroupBy(a => a.UserId).Select(a => new { userId = a.Key, email = a.FirstOrDefault().UserInformation.Email, fullName = a.FirstOrDefault().UserInformation.FullName }).ToList();
                    foreach (var i in preExpried)
                    {
                        try
                        {
                            var emailProfile = listPreExpired.Where(a => a.UserId == i.userId).ToList();
                            var isSend = _emailService.SendExpired(i.email, i.fullName, emailProfile, EmailType.PreExpired);
                            if (isSend)
                            {
                                _emailHistoryRepository.Create(new EmailHistory()
                                {
                                    Email = i.email,
                                    Type = EmailTypeHistory.PreExpired,
                                    DateSend = DateTime.Now
                                });
                                _unitOfWork.SaveChanges();
                            }
                        }
                        catch
                        { }
                    }
                }
                catch
                {

                }
            }
            foreach (var item in listExpired)
            {
                try
                {
                    var expried = listExpired.GroupBy(a => a.UserId).Select(a => new { userId = a.Key, email = a.FirstOrDefault().UserInformation.Email, fullName = a.FirstOrDefault().UserInformation.FullName }).ToList();
                    foreach (var i in expried)
                    {
                        try
                        {
                            var emailProfile = listExpired.Where(a => a.UserId == i.userId).ToList();
                            var isSend = _emailService.SendExpired(i.email, i.fullName, emailProfile, EmailType.Exprired);
                            if (isSend)
                            {
                                _emailHistoryRepository.Create(new EmailHistory()
                                {
                                    Email = i.email,
                                    Type = EmailTypeHistory.Exprired,
                                    DateSend = DateTime.Now
                                });
                                _unitOfWork.SaveChanges();
                            }
                        }
                        catch
                        { }
                    }
                }
                catch
                {

                }
            }
        }
    }

    public class JobScheduler
    {
        public static async Task Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<EmailJob>()
                            .WithIdentity("myJob", "group1")
                            .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(x => x
                .WithIntervalInHours(24)
                .OnEveryDay()
                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0)))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }


    }
}