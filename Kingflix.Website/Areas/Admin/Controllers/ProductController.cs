using ExcelDataReader;
using Kingflix.Domain.DomainModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.ViewModel;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public ProductController(
            IProductService productService,
            IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }
        public ActionResult Index()
        {
            var model = _productService.GetProductList();
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.Password = "";
            ViewBag.DateEnd = "";
            var product = _productService.GetProductList();
            if (product.Count() != 0)
            {
                ViewBag.Password = product.FirstOrDefault().Password;
                ViewBag.DateEnd = product.FirstOrDefault().DateEnd.ToString("dd/MM/yyyy");
            }
            if (Request.IsAjaxRequest())
                return PartialView("_CreateProductPartial");
            ViewBag.CategoryId = new SelectList(_productService.GetAllCategories(TypeOfAccount.KingflixAccount), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Password,Pincode,DateEnd,Cycle,Status,CategoryId")] Product product, List<ProfileViewModel> Profile)
        {
            var result = new ResultViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _productService.CreateProductAndProfile(product, Profile);
                    if (Request.IsAjaxRequest())
                    {
                        result.status = "success";
                        result.message = "Thành công! Đã tạo tài khoản";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    return RedirectToAction("Index");
                }
                catch
                {
                    if (Request.IsAjaxRequest())
                    {
                        result.status = "error";
                        result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
            }

            var productList = _productService.GetProductList();
            ViewBag.Password = "";
            if (productList.Count() != 0)
                ViewBag.Password = productList.FirstOrDefault().Password;
            ViewBag.CategoryId = new SelectList(_productService.GetAllCategories(TypeOfAccount.KingflixAccount), "CategoryId", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        public ActionResult GetCategoryOrtherList()
        {
            var model = _productService.GetAllCategories(TypeOfAccount.KingflixAccount).Where(a=>a.Type == TypeOfCategory.Orther).ToList();
            return PartialView("_CategoryListPartial", model);
        }

        [HttpPost]
        public ActionResult UploadProductList(HttpPostedFileBase upload)
        {
            var json = new ResultViewModel();
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;

                    IExcelDataReader reader = null;

                    string ext = Path.GetExtension(upload.FileName);

                    if (ext.EndsWith(".xls") || ext.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Tệp đưa lên không đúng định dạng");
                        return View();
                    }
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    reader.Close();
                    json = _productService.GetAndSaveProductDataFromExcel(result);
                    return Json(json, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            json.status = "error";
            json.message = "Lỗi. Vui lòng kiểm tra lại định dạng tệp và thử lại";
            return Json(json, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult ChangeProductStatus(string[] productList, ProductStatus status)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.ChangeProductListStatus(productList, status);
                result.status = "success";
                result.message = "Thay đổi trạng thái các sản phẩm thành công!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thay đổi trạng thái các sản phẩm thất bại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Edit(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return HttpNotFound();

            Product product = _productService.GetProductById(productId);
            if (product == null)
                return HttpNotFound();
            ViewBag.CategoryId = new SelectList(_productService.GetAllCategories(TypeOfAccount.KingflixAccount), "CategoryId", "Name", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Password,DateEnd,DateCreated,Status,CategoryId,Cycle")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_productService.GetAllCategories(TypeOfAccount.KingflixAccount), "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Delete(string productId)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.DeleteProduct(productId);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetProductReplaceList(string categoryId, string productReplaceId, string currentProductId)
        {
            var model = _productService.GetProductReplaceList(categoryId, currentProductId);
            ViewBag.ProductId = "-1";
            if (!string.IsNullOrEmpty(productReplaceId))
            {
                ViewBag.ProductId = productReplaceId;
            }
            return PartialView("_ProductListSelectPartial", model);
        }
        public ActionResult GetUserList(string userId)
        {
            var model = _userService.GetUserList();
            if (model.Count() != 0 && !string.IsNullOrEmpty(userId))
                foreach (var item in model)
                {
                    if (item.Id == userId)
                        item.IsSelected = true;
                }
            return PartialView("_UserListPartial", model);
        }
    }
}