using System;
using System.Linq;
using System.Collections.Generic;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using Kingflix.Services.Interfaces;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace Kingflix.Services
{
    public class EmailService : IEmailService
    {
        public EmailService() { }
        public EmailHistory SendExpired(string email, string fullName, List<Profile> profile, EmailType type)
        {
            if (type == EmailType.Custom)
                return null;
            var isSend = true;
            GMailer mailer = new GMailer
            {
                ToEmail = email,
                IsHtml = true,
                Subject = type == EmailType.PreExpired ? "Kingflix - Tài khoản sắp hết hạn" : "Kingflix - Tài khoản đã hết hạn",
                Body = string.Empty
            };
            try
            {
                var account = "";
                foreach (var item in profile)
                {
                    account += "<tr>" +
                                    "<td>" + item.Products.Categories.Name + "</td>" +
                                    "<td>" + item.Products.ProductId + "</td>" +
                                    "<td>" + item.Name + "</td>" +
                                    "<td style='color: red;'>" + item.DateEnd.ToString("dd/MM/yyyy") + "</td>" +
                               "</tr>";
                }

                string template = Properties.Resources.ExpriedProfile;
                template = template.Replace("{FullName}", fullName ?? email)
                                   .Replace("{Email}", email)
                                   .Replace("{Account}", account);
                mailer.Body = template;
                mailer.Send();
            }
            catch (Exception ex)
            {
                isSend = false;
            }
            return new EmailHistory()
            {
                Title = mailer.Subject,
                Email = mailer.ToEmail,
                Content = mailer.Body,
                TimeSend = DateTime.Now,
                Status = isSend ? EmailStatus.Success : EmailStatus.Failure
            };
        }
        public EmailHistory SendEmailCustom(string email, EmailTemplate emailTemplate)
        {
            GMailer mailer = new GMailer
            {
                ToEmail = email,
                Subject = emailTemplate.Title,
                Body = emailTemplate.Content,
                IsHtml = true
            };
            bool isSend = true;
            try
            {
                mailer.Send();
            }
            catch (Exception ex)
            {
                isSend = false;
            }
            return new EmailHistory()
            {
                Title = mailer.Subject,
                Email = mailer.ToEmail,
                Content = mailer.Body,
                TimeSend = DateTime.Now,
                Status = isSend ? EmailStatus.Success : EmailStatus.Failure
            };
        }
        public EmailHistory SendEmailForgotPassword(string email, string fullName, string resetPasswordLink)
        {
            GMailer mailer = new GMailer
            {
                ToEmail = email,
                Subject = "Yêu cầu tạo lại mật khẩu Kingflix.vn",
                IsHtml = true
            };
            bool isSend = true;
            try
            {
                var template = Properties.Resources.ForgotPassword;
                template = template.Replace("{FullName}", fullName ?? email)
                                    .Replace("{ResetPasswordLink}", resetPasswordLink);
                mailer.Body = template;
                mailer.Send();
            }
            catch (Exception ex)
            {
                isSend = false;
            }
            return new EmailHistory()
            {
                Title = mailer.Subject,
                Email = mailer.ToEmail,
                Content = mailer.Body,
                TimeSend = DateTime.Now,
                Status = isSend ? EmailStatus.Success : EmailStatus.Failure
            };
        }
        public EmailHistory SendOrder(Order order, List<Profile> profiles = null)
        {
            GMailer mailer = new GMailer
            {
                ToEmail = order.UserInformation.Email,
                IsHtml = true
            };
            bool isSend = true;
            try
            {
                string template = Properties.Resources.Order;
                //Thêm thông tin thanh toán - đơn hàng
                template = template.Replace("{LinkOrder}", "https://kingflix.vn/Order/Details?orderId=" + order.OrderId)
                                   .Replace("{OrderId}", order.OrderId)
                                   .Replace("{FullName}", order.UserInformation.FullName)
                                   .Replace("{Email}", order.UserInformation.Email)
                                   .Replace("{PhoneNumber}", order.UserInformation.PhoneNumber)
                                   .Replace("{Status}", order.Status.GetEnumDisplayName())
                                   .Replace("{DateCreate}", order.DateCreated.ToString("HH:mm:ss dd/MM/yyyy"));
                var subject = "";
                var productInformation = "";
                var listDetails = "";
                if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.WaitingForPay)
                {
                    if (order.Status == OrderStatus.WaitingForPay)
                        subject = "Kingflix.vn - Đơn hàng mới #" + order.OrderId;
                    else
                        subject = "Kingflix.vn - Thanh toán thành công #" + order.OrderId;
                    foreach (var item in order.OrderDetails)
                    {
                        listDetails += "<tr>" +
                                            "<td>" + item.Categories.Name + "</td>" +
                                            "<td>" + item.Month + "</td>" +
                                            "<td>" + item.Count + "</td>" +
                                            "<td>" + (item.IsGuarantee ? "Có" : "Không") + "</td>" +
                                            "<td>" + (item.IsKingflixAccount ? "Kingflix cấp" : "Khách hàng cấp") + "</td>" +
                                       "</tr>";
                    }
                    productInformation = Properties.Resources.OrderDetails.Replace("{OrderDetails}", listDetails);
                }
                else if (order.Status == OrderStatus.Done)
                {
                    subject = "Kingflix.vn - Hoàn tất đơn hàng #" + order.OrderId;
                    foreach (var item in profiles)
                    {
                        listDetails += "<tr>" +
                                        "<td>" + item.Products.Categories.Name + "</td>" +
                                        "<td>" + item.ProductId + "</td>" +
                                        "<td>" + item.Products.Password + "</td>" +
                                        "<td>" + item.Name + "</td>" +
                                        "<td>" + item.Pincode + "</td>" +
                                        "<td style='color: red;'>" + item.DateEnd.ToString("dd/MM/yyyy") + "</td>" +
                                   "</tr>";
                    }
                    productInformation = Properties.Resources.AccountList.Replace("{AccountList}", listDetails);
                }
                else if (order.Status == OrderStatus.Cancelled)
                {
                    subject = "Kingflix.vn - Hủy đơn #" + order.OrderId;
                }
                template = template.Replace("{ProductInformation}", productInformation);
                mailer.Subject = subject;
                mailer.Body = template;
                mailer.Send();
            }
            catch (Exception ex)
            {
                isSend = false;
            }
            return new EmailHistory()
            {
                Title = mailer.Subject,
                Email = mailer.ToEmail,
                Content = mailer.Body,
                TimeSend = DateTime.Now,
                Status = isSend ? EmailStatus.Success : EmailStatus.Failure
            };
        }
    }

    //public class EmailJob : IJob
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IRepository<Profile> _profileRepository;
    //    private readonly IRepository<EmailHistory> _emailHistoryRepository;
    //    private readonly IEmailService _emailService;
    //    public EmailJob(
    //        IUnitOfWork unitOfWork,
    //        IRepository<Profile> profileService,
    //        IRepository<EmailHistory> emailHistoryRepository,
    //        IEmailService emailService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _profileRepository = profileService;
    //        _emailHistoryRepository = emailHistoryRepository;
    //        _emailService = emailService;
    //    }
    //    public async Task Execute(IJobExecutionContext context)
    //    {
    //        var profiles = _profileRepository.GetAll();
    //        var listPreExpired = profiles.Where(a => a.DateEnd.AddDays(-7).Date == DateTime.Today.Date && !string.IsNullOrEmpty(a.UserId));
    //        var listExpired = profiles.Where(a => a.DateEnd.AddDays(1).Date == DateTime.Today.Date && !string.IsNullOrEmpty(a.UserId));
    //        foreach (var item in listPreExpired)
    //        {
    //            try
    //            {
    //                var preExpried = listPreExpired.GroupBy(a => a.UserId).Select(a => new { userId = a.Key, email = a.FirstOrDefault().UserInformation.Email, fullName = a.FirstOrDefault().UserInformation.FullName }).ToList();
    //                foreach (var i in preExpried)
    //                {
    //                    try
    //                    {
    //                        var emailProfile = listPreExpired.Where(a => a.UserId == i.userId).ToList();
    //                        var isSend = _emailService.SendExpired(i.email, i.fullName, emailProfile, EmailType.PreExpired);

    //                        _emailHistoryRepository.Create(new EmailHistory()
    //                        {
    //                            Email = i.email,
    //                            Type = EmailTypeHistory.PreExpired,
    //                            TimeSend = DateTime.Now
    //                        });
    //                        _unitOfWork.SaveChanges();
    //                    }
    //                    catch
    //                    { }
    //                }
    //            }
    //            catch
    //            {

    //            }
    //        }
    //        foreach (var item in listExpired)
    //        {
    //            try
    //            {
    //                var expried = listExpired.GroupBy(a => a.UserId).Select(a => new { userId = a.Key, email = a.FirstOrDefault().UserInformation.Email, fullName = a.FirstOrDefault().UserInformation.FullName }).ToList();
    //                foreach (var i in expried)
    //                {
    //                    try
    //                    {
    //                        var emailProfile = listExpired.Where(a => a.UserId == i.userId).ToList();
    //                        var isSend = _emailService.SendExpired(i.email, i.fullName, emailProfile, EmailType.Exprired);
    //                        if (isSend)
    //                        {
    //                            _emailHistoryRepository.Create(new EmailHistory()
    //                            {
    //                                Email = i.email,
    //                                Type = EmailTypeHistory.Exprired,
    //                                TimeSend = DateTime.Now
    //                            });
    //                            _unitOfWork.SaveChanges();
    //                        }
    //                    }
    //                    catch
    //                    { }
    //                }
    //            }
    //            catch
    //            {

    //            }
    //        }
    //    }
    //}

    //public class JobScheduler
    //{
    //    public static async Task Start()
    //    {
    //        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
    //        await scheduler.Start();


    //        IJobDetail job = JobBuilder.Create<EmailJob>()
    //                        .WithIdentity("myJob", "group1")
    //                        .Build();

    //        ITrigger trigger = TriggerBuilder.Create()
    //            .WithDailyTimeIntervalSchedule(x => x
    //            .WithIntervalInHours(24)
    //            .OnEveryDay()
    //            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0)))
    //            .Build();

    //        await scheduler.ScheduleJob(job, trigger);
    //    }
    //}
}