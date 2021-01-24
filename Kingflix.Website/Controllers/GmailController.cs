using System;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Data;

namespace Kingflix.Controllers
{
    public class GmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IOrderService _orderService;
        public GmailController(
            IEmailService emailService,
            IOrderService orderService
            )
        {
            _emailService = emailService;
            _orderService = orderService;
        }
        [HttpPost]
        public bool SendEmailOrderSuccess(string orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var isSendMail = _emailService.SendOrderPending(order);
            if (isSendMail)
                _emailService.AddEmailHistory(EmailTypeHistory.OrderSuccess, order.UserInformation.Email);               
            return isSendMail;
        }
    }
}