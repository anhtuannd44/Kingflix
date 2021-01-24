using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace Kingflix.Controllers
{
    public class JobsController : Controller
    {
        private readonly IBlogService _blogService;
        public JobsController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        public ActionResult Index(int? page)
        {
            var blog = _blogService.GetRecruimentList().Where(a=>a.Status == Status.Public);
            return View(blog.OrderByDescending(a => a.DateCreated).ToPagedList(page ?? 1, 12));
        }
        public ActionResult JobList(string url, int? page)
        {
            if (string.IsNullOrEmpty(url))
                return RedirectToAction("Index");
            var category = _blogService.GetRecruimentCategoryList().Where(a => a.Url == url);
            if (category.Count() == 0)
                return HttpNotFound();
            var blog = _blogService.GetRecruimentList().Where(a => a.BlogCategory.Url == url).ToList();
            ViewBag.CategoryName = category.FirstOrDefault().Name;
            return View(blog.OrderByDescending(a => a.DateCreated).ToPagedList(page ?? 1, 12));
        }
        public ActionResult Details(string url)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound();
            var product = _blogService.GetRecruimentList().Where(a => a.Url == url);
            if (product.Count() == 0)
                return HttpNotFound();
            var model = product.FirstOrDefault();
            return View(model);
        }

        public ActionResult GetBlogCategory()
        {
            var model = _blogService.GetRecruimentCategoryList();
            return PartialView("_CategoryListPartial", model);
        }
    }
}