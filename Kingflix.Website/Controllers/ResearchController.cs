using Kingflix.Services.Data;
using System.Web.Mvc;

namespace Kingflix.Controllers
{
    public class ResearchController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        // GET: Research
        public ActionResult Index(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = db.GoogleForm.Find(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }
    }
}