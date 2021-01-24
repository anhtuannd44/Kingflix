using Kingflix.Services.Data;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExtendController : Controller
    {
        
        
    }
}