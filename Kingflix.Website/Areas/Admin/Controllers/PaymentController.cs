using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public ActionResult Index()
        {
            var model = _paymentService.GetPaymentList();
            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Name,AccountNumber,AccountName,AccountAddress,Content,Description,ImageId,Type,Status")] Payment payment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                _paymentService.CreatePaymentMethod(payment, file);
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Admin/Payment/Edit/5
        public ActionResult Edit(int? paymentId)
        {
            if (paymentId == null)
                return HttpNotFound();
            Payment payment = _paymentService.GetPaymentById(paymentId);
            if (payment == null)
                return HttpNotFound();
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "PaymentId,Name,AccountNumber,AccountName,Content,AccountAddress,Description,ImageId,Type,Status")] Payment payment, HttpPostedFileBase logo)
        {
            if (ModelState.IsValid)
            {
                _paymentService.UpdatePaymentMethod(payment, logo);                
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Admin/Payment/Delete/5
        [HttpPost]
        public ActionResult Delete(int paymentId)
        {
            var result = new ResultViewModel();
            try
            {
                result = _paymentService.DeletePaymentMethod(paymentId);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại. Vui lòng thử lại";
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
