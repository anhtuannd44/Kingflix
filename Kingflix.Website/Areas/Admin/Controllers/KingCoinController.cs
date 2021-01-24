using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class KingCoinController : Controller
    {
        private readonly IKingCoinService _kingCoinService;
        public KingCoinController(
            IKingCoinService kingCoinService)
        {
            _kingCoinService = kingCoinService;
        }
        public ActionResult Index()
        {
            var kingCoin = _kingCoinService.GetKingCoinList();
            return View(kingCoin);
        }

        [HttpPost]
        public ActionResult UpdateKingCoin(string id, CoinStatus status)
        {
            var result = new ResultViewModel();
            try
            {
                _kingCoinService.UpdateKingCoin(id, status);
                result.status = "success";
                result.message = "Thành công! Đã duyệt nạp tiền của khách";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        // GET: Admin/KingCoin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KingCoin kingCoin = _kingCoinService.GetKingCoinItem(id);
            if (kingCoin == null)
            {
                return HttpNotFound();
            }
            return View(kingCoin);
        }

        // GET: Admin/KingCoin/Create
        public ActionResult Create()
        {
            ViewBag.PaymentId = new SelectList(_kingCoinService.GetListPayment(), "PaymentId", "Name");
            return View();
        }

        // POST: Admin/KingCoin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Price,DateCreated,DateModified,DateConfirm,Status,File,UserId,IsSendMail,Image,VoucherId,PaymentId,CancelNote")] KingCoin kingCoin)
        {
            if (ModelState.IsValid)
            {
                _kingCoinService.CreateKingCoin(kingCoin);
                return RedirectToAction("Index");
            }

            ViewBag.PaymentId = new SelectList(_kingCoinService.GetListPayment(), "PaymentId", "Name", kingCoin.PaymentId);
            return View(kingCoin);
        }
    }
}
