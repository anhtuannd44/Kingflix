using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class RecruitmentController : Controller
    {
        private readonly IBlogService _blogService;
        public RecruitmentController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        public ActionResult Index()
        {
            var model = _blogService.GetRecruimentList();
            return View(model);
        }

        // GET: Admin/BlogAdmin/Create
        public ActionResult Create()
        {
            ViewBag.BlogCategoryId = new SelectList(_blogService.GetRecruimentCategoryList(), "BlogCategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Title,Url,Description,MetaDescription,MetaKeyword,BlogContent,ImageId,BlogCategoryId,Status")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                _blogService.CreateBlog(blog);
                return RedirectToAction("Index");
            }
            ViewBag.BlogCategoryId = new SelectList(_blogService.GetRecruimentCategoryList(), "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Blog blog = _blogService.GetBlogById(id);
            if (blog == null)
                return HttpNotFound();
            if (blog.BlogCategory.Type != BlogType.Recruitment)
                return HttpNotFound();
            ViewBag.BlogCategoryId = new SelectList(_blogService.GetRecruimentCategoryList(), "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }

        // POST: Admin/BlogAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "BlogId,Title,Url,Description,MetaDescription,MetaKeyword,BlogContent,ImageId,DateCreated,BlogCategoryId,Status")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                _blogService.UpdateBlog(blog);
                return RedirectToAction("Index");
            }
            ViewBag.BlogCategoryId = new SelectList(_blogService.GetRecruimentCategoryList(), "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }

        [HttpPost]
        public ActionResult MoveToTrash(int id)
        {
            var result = new ResultViewModel();
            try
            {
                var item = _blogService.GetBlogById(id);
                item.Status = Status.Trash;
                _blogService.UpdateBlog(item);
                result.status = "success";
                result.message = "Thành công! Đã chuyển vào thùng rác";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _blogService.DeleteBlog(id);
                result.status = "success";
                result.message = "Thành công! Đã xóa bài viết hoàn toàn";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Category()
        {
            return View(_blogService.GetRecruimentCategoryList().ToList());
        }

        [HttpPost]
        public ActionResult CreateBlogCategory(string BlogCategoryName)
        {
            var result = new ResultViewModel();
            try
            {
                _blogService.CreateBlogCategory(BlogCategoryName, BlogType.Recruitment);
                result.status = "success";
                result.message = "Thành công! Đã thêm danh mục bài viết.";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult EditBlogCategory(int BlogCategoryId, string BlogCategoryNameEdit)
        {
            var result = new ResultViewModel();
            try
            {
                _blogService.UpdateBlogCategory(BlogCategoryId, BlogCategoryNameEdit);
                result.status = "success";
                result.message = "Thành công! Đã chỉnh sửa danh mục.";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult RemoveBlogCategory(int BlogCategoryId)
        {
            var result = new ResultViewModel();
            try
            {
                _blogService.DeleteBlogCategory(BlogCategoryId);
                result.status = "success";
                result.message = "Thành công! Đã xóa danh mục bài viết.";
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
