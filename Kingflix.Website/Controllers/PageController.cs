using Kingflix.Services.Data;
using System.Linq;
using System.Web.Mvc;

namespace Kingflix.Controllers
{
    public class PageController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(string url)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound();
            var product = db.Page.Where(a => a.Url == url).ToList();
            if (product.Count == 0)
                return HttpNotFound();
            var model = product.FirstOrDefault();
            return View(model);
        }
    }
}