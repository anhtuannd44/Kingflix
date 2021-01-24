using Kingflix.Domain.ViewModel;
using Kingflix.Models;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Data;
using Kingflix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;

namespace Kingflix.Controllers
{
    public class ImageController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: Image
        public JsonResult UploadImage(HttpPostedFileBase File)
        {
            UploadImageViewModel result = new UploadImageViewModel();
            try
            {
                var name = Regex.Replace(HelperFunction.ConvertToNonUTF(File.FileName.Split('.')[0]), @"[^0-9a-zA-Z]+", "-");
                var ext = Path.GetExtension(File.FileName);
                name = _imageService.CheckImageExist(name, ext);

                db.Image.Add(new Image() { ImageId = name + ext, DateCreated = DateTime.Now });
                db.SaveChanges();
                string path = Path.Combine(Server.MapPath("~/Content/Upload/Images/"), name + ext);
                File.SaveAs(path);
                result.status = "ok";
                result.path = path;

            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditImage(string currentImageId, string newImageId)
        {
            var result = new ResultViewModel();
            try
            {
                var image = db.Image.Find(currentImageId);
                if (image != null)
                {
                    var ext = currentImageId.Split('.')[1];

                    newImageId = _imageService.CheckImageExist(newImageId, ext);

                    var path = Server.MapPath("~/Content/Upload/Images/");
                    System.IO.File.Move(path + currentImageId, path + newImageId + ext);

                    result.status = "success";
                    result.message = "Thành công! Hình ảnh đã được đổi tên";
                }
                else
                {
                    result.status = "error";
                    result.message = "Thất bại! Vui lòng thử lại";
                }
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteImage(string imageId)
        {
            var result = new ResultViewModel();
            try
            {
                var deleteImageInDbStatus = _imageService.RemoveImageInDb(imageId);
                if (deleteImageInDbStatus == 1)
                {
                    string fullPath = Request.MapPath("/Content/Upload/Images/" + imageId);
                    _imageService.RemoveImageInServer(fullPath);
                }
                result.status = "success";
                result.message = "Xóa hình ảnh thành công";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult LoadLibraryImage(bool IsMultiple, string[] selectedImage, string notLoadAll, string sortBy)
        {
            var model = db.Image.OrderByDescending(a => a.DateCreated).ToList();

            if (selectedImage != null)
            {
                foreach (var item in model)
                {
                    foreach (var select in selectedImage)
                    {
                        if (select == item.ImageId)
                            item.IsSelected = true;
                    }
                }
            }

            ViewBag.IsMultiple = IsMultiple.ToString();

            if (notLoadAll == "notLoadAll")
                return PartialView("_ImageLibraryPartial", model);

            return PartialView("_ImageLibraryModalPartial", model);
        }


        public ActionResult LoadSelectImage(string[] imageAlbum)
        {
            List<Image> model = new List<Image>();
            foreach (var item in imageAlbum)
            {
                var addItem = new Image()
                {
                    ImageId = item
                };
                model.Add(addItem);
            }
            return PartialView("_ImageSelectedPartial", model);
        }
    }
}