using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        public ActionResult Index(ReviewStatus? status)
        {
            var model = _reviewService.GetReviewList();
            if (status.HasValue)
                model = model.Where(a => a.Status == status.Value).ToList();

            ViewBag.Status = status;
            return View(model.OrderByDescending(a=>a.DateCreated).ToList());
        }

        [HttpPost]
        public ActionResult ChangeStatus(int id, ReviewStatus status)
        {
            var result = new ResultViewModel();
            try
            {
                var item = _reviewService.GetReviewById(id);
                item.Status = status;
                _reviewService.UpdateReview(item);
                result.status = "success";
                result.message = "Thành công! Đã cập nhật bình luận - đánh giá";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình duyệt. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Review review = _reviewService.GetReviewById(id);
            if (review == null)
                return HttpNotFound();
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewId,Content,Star,DateCreated,DateConfirm,ReplyFor,UserId,Image,Status")] Review review)
        {
            if (ModelState.IsValid)
            {
                _reviewService.UpdateReview(review);
                return RedirectToAction("Index");
            }
            return View(review);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _reviewService.DeleteReview(id);
                result.status = "success";
                result.message = "Thành công! Đã xóa bình luận - đánh giá đã chọn";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
