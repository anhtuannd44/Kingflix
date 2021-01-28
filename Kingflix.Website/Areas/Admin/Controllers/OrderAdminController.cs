using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using System.Collections.Generic;

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
            ViewBag.IsAcceptPayment = isAcceptPayment ? "True" : "False";
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
        public ActionResult Edit([Bind(Include = "OrderId,Month,Price,Profile,Status,CategoryId,CancelNote,DateCreated")] Order order, List<OrderDetails> OrderDetails)
        {
            order.OrderDetails = OrderDetails;
            var updateStatus = order.Status;
            ResultViewModel result = new ResultViewModel();
            order.Status = order.Status == OrderStatus.Done ? OrderStatus.Paid : order.Status;

            if (ModelState.IsValid)
            {
                var isSuccess = _orderService.EditOrder(order);
                if (isSuccess)
                {
                    result.status = "success";
                    result.message = "Đã lưu Order thành công!";
                }   
                if (updateStatus == OrderStatus.Done)
                    result = _orderService.UpdateOrder(order.OrderId, order.Status, order.CancelNote, false);
            }
            else
            {
                result.status = "error";
                result.message = "Lỗi nhập dữ liệu. Vui lòng kiểm tra lại dữ liệu chỉnh sửa và thử lại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult UpdateOrder(string orderId, OrderStatus status, string cancelNote)
        {
            var result = _orderService.UpdateOrder(orderId, status, cancelNote, true);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}