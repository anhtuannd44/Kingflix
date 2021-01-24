using Kingflix.Models;
using Kingflix.Domain.DomainModel;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Services.Data;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;

namespace Kingflix.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        public ActionResult ReviewInformation()
        {
            var review = db.Review.Where(a => a.Status == ReviewStatus.Accept).ToList();
            var model = new RevieViewModel();
            double sum = 0;
            double count = 0;
            for (int i = 1; i <= 5; i++)
            {
                var itemCount = review.Where(a => a.Star == i).Count();
                model.Data.Add(new EachStarViewModel()
                {
                    ReviewCount = review.Where(a => a.Star == i).Count(),
                    StarType = i,
                    ReviewPercent = Math.Round((double)itemCount / (double)review.Count, 4) * 100
                });
                sum += i * itemCount;
                count += itemCount * 5;
            }
            model.StarAvg = Math.Round((sum / count)*5, 1);
            if (Double.IsNaN(model.StarAvg))
                model.StarAvg = 0;
            return PartialView("_ReviewInformationPartial", model);
        }

        [HttpPost]
        public ActionResult SendReview(double? starCount, string contentReview, int? replyFor)
        {
            var result = new ResultViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                using (var dbs = new AppDbContext())
                {
                    var order = dbs.Order.Where(a => a.Status == OrderStatus.Done && a.UserId == userId);
                    if (order == null)
                        starCount = 0;
                }
                db.Review.Add(new Review()
                {
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    Star = starCount ?? 0,
                    ReplyFor = replyFor,
                    Content = contentReview,
                    Status = ReviewStatus.Pending
                });
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Bình luận của bạn đã được gửi và sẽ hiển thị khi được duyệt!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thắt bại! Bình luận của bạn chưa được lưu. Vui lòng thử lại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult LoadReviewComment()
        {
            var model = db.Review.Where(a => a.Status == ReviewStatus.Accept).OrderByDescending(a => a.DateCreated).ToList();
            return PartialView("_ReviewListPartial", model);
        }

        [HttpPost]
        public ActionResult LikeReview(int id)
        {
            var result = new ResultViewModel();
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var review = db.Review.Find(id);
                    review.Like++;
                    db.Entry(review).State = EntityState.Modified;
                    db.SaveChanges();
                    result.status = "success";
                    result.message = "Thành công! Cảm ơn bạn đã đánh giá bình luận";
                    result.likecount = review.Like.ToString();
                }
                catch
                {
                    result.status = "error";
                    result.message = "Thất bại! Có lỗi trong quá trình gửi dữ liệu. Vui lòng thử lại sau!";
                }
            }    
            else
            {
                result.status = "error";
                result.message = "Thất bại! Bạn cần đăng nhập để thực hiện hành động";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}