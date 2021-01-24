using Kingflix.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.ViewModel;
using Kingflix.Utilities;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "Admin")]
    [RoutePrefix("Account")]
    [Route("{action}")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IProductService _categoryService;
        public CategoryController(
            IProductService categoryService)
        {
            _categoryService = categoryService;
        }
        public ActionResult Index()
        {
            var model = _categoryService.GetAllCategories().OrderBy(a => a.DateCreated).ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Name,Type,TypeOfAccount,Details,Service,PromotionInfo,ImageId,GuaranteePrice,Guarantee")] Category category, double?[] Month, double?[] Price, double?[] Prices, bool[] IsShow, string[] TextFixed)
        {
            if (!ModelState.IsValid)
                return View(category);
            category.CategoryId = _categoryService.GenerateCategoryId();

            try
            {
                category.DateCreated = DateTime.Now;
                List<Price> pricesList = new List<Price>();
                if (Month != null)
                {
                    for (int i = 0; i < Month.Length - 1; i++)
                        for (int j = i + 1; j < Month.Length; j++)

                            if (Month[i] == Month[j])
                                Month[j] = null;
                    for (int i = 0; i < Month.Length; i++)
                        if (Month[i] != null && Price[i] != null)
                            pricesList.Add(new Price()
                            {
                                CategoryId = category.CategoryId,
                                Month = Month[i].Value,
                                SetPrice = Price[i].Value, //Giá gốc
                                IsShow = IsShow[i],
                                Prices = Prices[i], //Giá Khuyến mãi nếu có
                                TextFixed = TextFixed[i]
                            });
                }
                _categoryService.CreateCategory(category, pricesList);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(category);
            }
        }

        public ActionResult Edit(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return HttpNotFound();
            }
            Category category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CategoryId,Name,Type,DateCreated,Details,Status,Service,PromotionInfo,TypeOfAccount,ImageId,DateOrderAccept,DaysGuarantee,Color,ColorSecondary,ColorShadow,,GuaranteePrice,Guarantee")] Category category, int?[] Month, double?[] Price, double?[] Prices, bool[] IsShow, string[] TextFixed)
        {
            if (ModelState.IsValid)
            {
                category.DateModified = DateTime.Now;
                //AddPrice
                List<Price> pricesList = new List<Price>();
                if (Month != null)
                {
                    for (int i = 0; i < Month.Length - 1; i++)
                        for (int j = i + 1; j < Month.Length; j++)
                        {
                            if (Month[i] == Month[j])
                                Month[j] = null;
                        }
                    for (int i = 0; i < Month.Length; i++)
                    {
                        if (Month[i] != null && Price[i] != null)
                        {
                            pricesList.Add(new Price()
                            {
                                CategoryId = category.CategoryId,
                                Month = Month[i].Value,
                                SetPrice = Price[i].Value,
                                IsShow = IsShow[i],
                                Prices = Prices[i],
                                TextFixed = TextFixed[i]
                            });
                        }
                    }
                }
                _categoryService.UpdateCategory(category, pricesList);
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public ActionResult EditCategoryDetails()
        {
            var model = _categoryService.GetCategoryNetflixDetails(false); //Get All
            if (model.Count == 0)
            {
                model = _categoryService.GenerateCategoryNetflixDetails();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategoryDetails(List<CategoryNetflixDetails> CategoryNetflixDetails)
        {
            var result = new ResultViewModel();
            try
            {
                _categoryService.UpdateCategoryDetails(CategoryNetflixDetails);
                result.status = "success";
                result.message = "Thành công! Đã cập nhật dữ liệu";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra trong quá trình lưu, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var result = new ResultViewModel();
            try
            {
                if (id == Const.NETFLIX0 || id == Const.NETFLIX1 || id == Const.NETFLIX2 || id == Const.NETFLIX3)
                {
                    result.status = "error";
                    result.message = "Thất bại! Bạn không thể xóa Netflix";
                }
                else
                {
                    _categoryService.RemoveCategory(id);
                    result.status = "success";
                    result.message = "Thành công! Đã xóa loại tài khoản hoàn toàn";
                }
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