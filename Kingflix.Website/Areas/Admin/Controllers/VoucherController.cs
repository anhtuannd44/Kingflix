using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VoucherController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        public VoucherController(
            IPromotionService promotionService,
            IOrderService orderService,
            IProductService productService
            )
        {
            _promotionService = promotionService;
            _orderService = orderService;
            _productService = productService;
        }
        public ActionResult Index(string Status)
        {
            var model = _promotionService.GetVoucherList();
            return View(model);
        }

        [HttpPost]
        public string CheckVoucherId(string voucherId)
        {
            var voucher = _promotionService.GetVoucherById(voucherId);
            if (voucher != null)
                return "found";
            var voucherChild = _promotionService.GetVoucherChildById(voucherId);
            if (voucherChild != null)
                return "found";
            return "notfound";
        }

        public ActionResult CreateVoucher()
        {
            return PartialView("_CreateVoucherPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VoucherId,Name,Value,MaxMoney,VoucherFor,DateEnd,Type,Status,PolicyContent")] Voucher voucher, string[] VoucherCategory)
        {
            var result = new ResultViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _promotionService.CreateVoucher(voucher, VoucherCategory);
                    result.status = "success";
                    result.message = "Thành công! Voucher của bạn đã được tạo";
                }
                catch
                {
                    result.status = "error";
                    result.message = "Có lỗi xả ra khi tạo Voucher";
                }
            }
            else
            {
                result.status = "error";
                result.message = "Có lỗi xả ra khi tạo Voucher";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult EditVoucher(string voucherId)
        {
            var model = _promotionService.GetVoucherById(voucherId);
            ViewBag.Category = _productService.GetAllCategories();
            return PartialView("_EditVoucherPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "VoucherId,Name,Value,MaxMoney,DateEnd,VoucherFor,Type,Status,DateCreated,PolicyContent")] Voucher voucher, string[] VoucherCategory)
        {
            var result = new ResultViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _promotionService.UpdateVoucher(voucher, VoucherCategory);
                    result.status = "success";
                    result.message = "Thành công! Voucher của bạn đã được chỉnh sửa";
                }
                catch
                {
                    result.status = "error";
                    result.message = "Có lỗi xả ra khi sửa Voucher";
                }
            }
            else
            {
                result.status = "error";
                result.message = "Có lỗi xả ra khi sửa Voucher";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var result = new ResultViewModel();
            var orderHasVoucher = _orderService.GetOrderList(a => a.VoucherId == id && a.Status == OrderStatus.WaitingForPay);
            if (orderHasVoucher.Count() != 0)
            {
                result.status = "error";
                result.message = "Thất bại! Voucher này đang có người Order";
            }
            else
            {
                try
                {
                    _promotionService.DeleteVoucher(id);
                    result.status = "success";
                    result.message = "Thành công! Đã xóa Voucher";
                }
                catch
                {
                    result.status = "error";
                    result.message = "Thất bại! Có lỗi trong quá trình xóa Voucher";
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult VoucherChildList(string voucherId)
        {
            var voucher = _promotionService.GetVoucherById(voucherId);
            if (voucher == null)
                return HttpNotFound();
            var model = _promotionService.GetVoucherChildList(a => a.VoucherId == voucherId).ToList();
            ViewBag.VoucherId = voucherId;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateChildVoucher(int count, string voucherId)
        {
            var result = new ResultViewModel();
            try
            {
                _promotionService.CreateChildVoucher(count, voucherId);
                result.status = "success";
                result.message = "Thành công! Các mã con đã được tạo";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Chưa tạo được mã con. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
