using System;
using System.Web.Mvc;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class HomepagesController : Controller
    {
        private readonly ISettingService _settingService;
        public HomepagesController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Slider()
        {
            var model = _settingService.GetListHomepageSlider();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddSlider(string ImageId, string Title, string Content, string subContent, string YoutubeUrl, string contentBackground, string titleBackground, string subContentBackground, string TextFixed)
        {
            var result = new ResultViewModel();
            if (string.IsNullOrEmpty(ImageId) && string.IsNullOrEmpty(YoutubeUrl))
            {
                result.status = "error";
                result.message = "Thất bại! Bạn chưa chọn hình hoặc Video";
            }
            else
            {
                try
                {
                    result = _settingService.AddSlider(ImageId, Title, Content, subContent, YoutubeUrl, contentBackground, titleBackground, subContentBackground, TextFixed);
                }
                catch
                {
                    result.status = "error";
                    result.message = "Thất bại! Chưa thể thêm hình cho slider, vui lòng thử lại";
                }

            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult EditSlider(int id, string title, string content, string imageId, string subcontent, string youtubeUrl, string contentBackground, string titleBackground, string subContentBackground, string textFixed)
        {
            var result = new ResultViewModel();
            try
            {
                result = _settingService.EditSider(id, title, content, imageId, subcontent, youtubeUrl, contentBackground, titleBackground, subContentBackground, textFixed);
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Thất bại! Chưa hoàn tất chỉnh sửa. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult DeleteSlider(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _settingService.DeleteSlider(id);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình xử lý. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult SmallPicture()
        {
            var model = _settingService.GetSmallPicture();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSmallPicture(int id, string title, string content, string imageId, string backgroundColor)
        {
            var result = new ResultViewModel();
            try
            {
                _settingService.EditSmallPicture(id, title, content, imageId, backgroundColor);
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Thất bại! Chưa hoàn tất chỉnh sửa. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult SetupShop()
        {
            _settingService.CreateShopBannerIfNullOrNotOnly();
            var model = _settingService.GetBannerShopList();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddSliderShop(string ImageId, string Link)
        {
            var result = new ResultViewModel();
            if (string.IsNullOrEmpty(ImageId))
            {
                result.status = "error";
                result.message = "Bạn chưa chọn hình nào cả";
            }
            else
            {
                try
                {
                    _settingService.AddSliderShop(ImageId, Link);
                    result.status = "success";
                    result.message = "Thành công! Bạn đã thêm hình vào Slider Shop";
                }
                catch
                {
                    result.status = "error";
                    result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult AddBannerShop(int Id, string imageId, string link)
        {
            var result = new ResultViewModel();
            try
            {
                _settingService.AddBannerShop(Id, imageId, link);
                result.status = "success";
                result.message = "Thành công! Đã cập nhật Banner";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult Social()
        {
            _settingService.CreateSocialIfNullOrNotOnly();
            var model = _settingService.GetSociallist();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Social(string Youtube, string Facebook, string Instagram)
        {
            if (!string.IsNullOrEmpty(Youtube) && !string.IsNullOrEmpty(Facebook) && !string.IsNullOrEmpty(Instagram))
            {
                _settingService.UpdateSocial(Youtube, Facebook, Instagram);
            }
            return RedirectToAction("Social");
        }
    }
}
