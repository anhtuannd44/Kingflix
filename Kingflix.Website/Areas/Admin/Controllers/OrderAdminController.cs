using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderAdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<Category> _categoryRepository;

        public OrderAdminController(
            IOrderService orderService,
            IRepository<Category> categoryRepository)
        {
            _orderService = orderService;
            _categoryRepository = categoryRepository;
        }

        // GET: Admin/OrderAdmin
        public ActionResult Index(OrderStatus? status, bool isAcceptPayment = true)
        {
            var model = _orderService.GetOrderWithUserNotNull(status, isAcceptPayment);
            ViewBag.Status = status;
            return View(model);
        }

        public ActionResult Edit(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return HttpNotFound();

            var model = _orderService.GetOrderById(orderId);
            if (model == null)
                return HttpNotFound();
            ViewData["CategoryId"] = _categoryRepository.GetAll().ToList();
            ViewBag.Noti = "";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,Month,Price,Profile,Status,CategoryId,CancelNote")] Order order)
        {
            var orderItem = _orderService.GetOrderById(order.OrderId);
            orderItem.Price = order.Price;
            ResultViewModel result = new ResultViewModel();
            if (ModelState.IsValid)
            {
                result = _orderService.UpdateOrder(order.OrderId, order.Status, order.CancelNote, false);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult UpdateOrder(string orderId, OrderStatus status, string cancelNote)
        {
            var result = _orderService.UpdateOrder(orderId, status, cancelNote, true);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public int GetOrderNoti()
        {
            return _orderService.GetOrderNotiCount();
        }
    }
}