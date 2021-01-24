using Kingflix.Models;
using Kingflix.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Data;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class BaoTriController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        // GET: Admin/BaoTri
        public ActionResult Index()
        {
            var model = db.Setting.Find("BaoTri");
            if (model == null)
            {
                var baotri = db.Setting.Add(new Setting()
                {
                    SettingId = "BaoTri",
                    Title = "BẢO TRÌ HỆ THỐNG!",
                    Content = "Xin lỗi quý khách vì sự bất tiện này, vui lòng quay lại sau ít phút.",
                    Status = Status.Public
                });
                db.SaveChanges();
                model = baotri;
            }
            return View("~/Areas/Admin/Views/BaoTri/Index.cshtml", model);
        }
    }
}