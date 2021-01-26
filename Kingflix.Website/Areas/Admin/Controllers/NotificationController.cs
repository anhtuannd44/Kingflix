using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NotificationController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Support> _supportRepository;
        private readonly IRepository<Review> _reviewRepository;
        public NotificationController(
            IRepository<Order> orderRepository,
            IRepository<Support> supportRepository,
            IRepository<Review> reviewRepository)
        {
            _orderRepository = orderRepository;
            _supportRepository = supportRepository;
            _reviewRepository = reviewRepository;
        }
        public ActionResult Index()
        {
            var model = GetListNoti().OrderByDescending(a => a.DateCreated).ToList();
            return View(model);
        }
        public ActionResult GetNotificationAdmin()
        {
            var noti = GetListNoti().OrderByDescending(a => a.DateCreated).Take(5).ToList();
            return PartialView("_NotiPartial", noti);
        }

        List<NotificationViewModel> GetListNoti()
        {
            var order = _orderRepository.Filter(a => a.Status == OrderStatus.WaitingForPay).ToList();
            var support = _supportRepository.Filter(a => a.Status == SupportStatus.Pending).ToList();
            List<NotificationViewModel> noti = new List<NotificationViewModel>();
            foreach (var item in order)
            {
                noti.Add(new NotificationViewModel()
                {
                    Title = "Đơn mới từ " + item.UserInformation.FullName,
                    DateCreated = item.DateCreated,
                    Type = NotificationType.Order,
                    Link = Url.Action("Edit","OrderAdmin", new { orderId = item.OrderId })
                });
            }
            foreach (var item in support)
            {
                noti.Add(new NotificationViewModel()
                {
                    Title = item.Title,
                    Content = item.Content,
                    DateCreated = item.DateCreate,
                    Type = NotificationType.Error,
                    Link = Url.Action("Edit", "Supports", new { id = item.SupportId })
                });
            }
            return noti.OrderByDescending(a=>a.DateCreated).ToList();
        }
        [HttpPost]
        public int GetNotiReview()
        {
            return _reviewRepository.Filter(a => a.Status == ReviewStatus.Pending).Count();
        }
        [HttpPost]
        public int GetUserManageNoti()
        {
            return _supportRepository.Filter(a => a.Status == SupportStatus.Pending).Count();
        }
    }
}