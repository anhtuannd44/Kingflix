using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Manage.Controllers
{
    [Authorize]
    public class OrderHistoryController : Controller
    {
        // GET: Manage/OrderHistory
        private readonly IOrderService _orderService;
        public OrderHistoryController
            (
                IOrderService orderService
            )
        {
            _orderService = orderService;
        }
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = _orderService.GetOrderList().Where(a => a.UserId == userId && a.Type == OrderType.Order).OrderByDescending(a => a.DateCreated).ToList();
            return View(model);
        }
    }
}