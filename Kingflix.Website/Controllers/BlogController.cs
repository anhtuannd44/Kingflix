using Kingflix.Domain.Enumerables;
using Kingflix.Services.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace Kingflix.Controllers
{
    public class BlogController : Controller
    {

        private AppDbContext db = new AppDbContext();
        public ActionResult Index(int? page)
        {
            var blog = db.Blog.Where(a=>a.BlogCategory.Type == BlogType.Blog);
            return View(blog.OrderByDescending(a => a.DateCreated).ToPagedList(page ?? 1, 12));
        }
        public ActionResult BlogList(string url, int? page)
        {
            if (string.IsNullOrEmpty(url))
                return RedirectToAction("Index");
            var category = db.BlogCategory.Where(a => a.Url == url && a.Type == BlogType.Blog);
            if (category.Count() == 0)
                return HttpNotFound();
            var blog = db.Blog.Where(a=>a.BlogCategory.Url == url && a.BlogCategory.Type == BlogType.Blog).ToList();
            ViewBag.CategoryName = category.FirstOrDefault().Name;
            return View(blog.OrderByDescending(a => a.DateCreated).ToPagedList(page ?? 1, 12));
        }
        public ActionResult Details(string url)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound();
            var product = db.Blog.Where(a => a.Url == url && a.BlogCategory.Type == BlogType.Blog).ToList();
            if (product.Count == 0)
                return HttpNotFound();
            var model = product.FirstOrDefault();
            return View(model);
        }

        public ActionResult GetBlogCategory()
        {
            var model = db.BlogCategory.Where(a=>a.Type == BlogType.Blog).ToList();
            return PartialView("_CategoryListPartial", model);
        }
    }
}