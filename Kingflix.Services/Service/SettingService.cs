using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Kingflix.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Homepage> _homepageRepository;
        private readonly IRepository<Image> _imageRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Setting> _settingRepository;
        public SettingService(
            IUnitOfWork unitOfWork,
            IRepository<Homepage> homepageRepository,
            IRepository<Image> imageRepository,
            IRepository<Question> questionRepository,
            IRepository<Setting> settingRepository
            )
        {
            _unitOfWork = unitOfWork;
            _homepageRepository = homepageRepository;
            _imageRepository = imageRepository;
            _questionRepository = questionRepository;
            _settingRepository = settingRepository;
        }
        public List<Homepage> GetListHomepageSlider()
        {
            return _homepageRepository.Filter(a => a.Type == PictureHomeType.Slider).ToList();
        }
        public ResultViewModel AddSlider(string ImageId, string Title, string Content, string subContent, string YoutubeUrl, string contentBackground, string titleBackground, string subContentBackground, string TextFixed)
        {
            var result = new ResultViewModel();
            Homepage slider = new Homepage()
            {
                Title = Title,
                Content = Content,
                SubContent = subContent,
                Type = PictureHomeType.Slider,
                ContentColor = contentBackground,
                TitleColor = titleBackground,
                SubContentColor = subContentBackground,
                TextFixed = TextFixed
            };
            if (!string.IsNullOrEmpty(YoutubeUrl))
            {
                slider.YoutubeId = HelperFunction.GetYoutubeVideoId(YoutubeUrl);
            }
            else
            {
                var image = _imageRepository.Find(ImageId);
                if (image == null)
                {
                    result.status = "error";
                    result.message = "Thăt bại! Vui lòng thử lại";
                    return result;
                }
            }
            _homepageRepository.Create(slider);
            _unitOfWork.SaveChanges();
            return result;
        }
        public ResultViewModel EditSider(int id, string title, string content, string imageId, string subcontent, string youtubeUrl, string contentBackground, string titleBackground, string subContentBackground, string textFixed)
        {
            var result = new ResultViewModel();
            var slider = _homepageRepository.Find(id);
            if (slider == null)
            {
                result.status = "error";
                result.message = "Thất bại! Dữ liệu không tồn tại. Vui lòng tải lại trang";
                return result;
            }

            if (slider.Type != PictureHomeType.Slider)
            {
                result.status = "error";
                result.message = "Thất bại! Không đúng dữ liệu, vui lòng tải lại trang";
            }
            else
            {
                if (string.IsNullOrEmpty(youtubeUrl))
                {
                    slider.Title = title;
                    slider.Content = content;
                    slider.ImageId = imageId;
                    slider.SubContent = subcontent;
                    slider.YoutubeId = string.Empty;
                    slider.TitleColor = titleBackground;
                    slider.ContentColor = contentBackground;
                    slider.SubContentColor = subContentBackground;
                    slider.TextFixed = textFixed;
                }
                else
                {
                    slider.YoutubeId = HelperFunction.GetYoutubeVideoId(youtubeUrl);
                    slider.TextFixed = textFixed;

                }
                _homepageRepository.Update(slider);
                _unitOfWork.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã chỉnh sửa slider";
            }
            return result;
        }
        public ResultViewModel DeleteSlider(int id)
        {
            var result = new ResultViewModel();
            var slider = _homepageRepository.Find(id);
            if (slider == null)
            {
                result.status = "error";
                result.message = "Thất bại! Không tồn tại dữ liệu. Vui lòng tải lại trang";
            }
            else
            {
                if (slider.Type != PictureHomeType.Slider && slider.Type != PictureHomeType.SliderCategory)
                {
                    result.status = "error";
                    result.message = "Thất bại! Không đúng dữ liệu. Vui lòng tải lại trang";
                }
                else
                {
                    _homepageRepository.Delete(slider);
                    _unitOfWork.SaveChanges();
                    result.status = "success";
                    result.message = "Thành công! Đã xóa Slider khỏi hệ thống";
                }
            }
            return result;
        }
        public List<Homepage> GetSmallPicture()
        {
            var smallPictures = _homepageRepository.Filter(a => a.Type == PictureHomeType.SmallPicture);
            if (smallPictures.Count() != 3)
            {
                if (smallPictures.Count() > 0)
                {
                    foreach (var item in smallPictures)
                    {
                        _homepageRepository.Delete(item);
                    }
                }
                for (int i = 1; i <= 3; i++)
                {
                    _homepageRepository.Create(new Homepage()
                    {
                        Title = "Title " + i,
                        Content = "Content " + i,
                        Type = PictureHomeType.SmallPicture,
                        OrderBy = i,
                        BackgroundColor = "#FFFFFF"
                    });
                }
                _unitOfWork.SaveChanges();
                smallPictures = _homepageRepository.Filter(a => a.Type == PictureHomeType.SmallPicture);
            }
            return smallPictures.ToList();
        }
        public ResultViewModel EditSmallPicture(int id, string title, string content, string imageId, string backgroundColor)
        {
            var result = new ResultViewModel();
            var smallPicture = _homepageRepository.Find(id);
            if (smallPicture == null)
            {
                result.status = "error";
                result.message = "Thất bại! Dữ liệu không tồn tại. Vui lòng tải lại trang";
            }
            else
            {
                if (smallPicture.Type != PictureHomeType.SmallPicture)
                {
                    result.status = "error";
                    result.message = "Thất bại! Không đúng dữ liệu, vui lòng tải lại trang";
                }
                else
                {
                    smallPicture.Title = title;
                    smallPicture.Content = content;
                    smallPicture.ImageId = string.IsNullOrEmpty(imageId) ? null : imageId;
                    smallPicture.BackgroundColor = backgroundColor;
                    _homepageRepository.Update(smallPicture);
                    _unitOfWork.SaveChanges();
                    result.status = "success";
                    result.message = "Thành công! Đã chỉnh sửa content trang chủ";
                }
            }
            return result;
        }
        public void CreateShopBannerIfNullOrNotOnly()
        {
            for (int i = 1; i <= 6; i++)
            {

                var item = _homepageRepository.Filter(a => a.Type == PictureHomeType.BannerShop && a.Title == "BANNER" + i.ToString());
                if (item.Count() > 1 || item.Count() == 0)
                {
                    if (item.Count() > 1)
                        foreach (var j in item)
                            _homepageRepository.Delete(j);
                    _homepageRepository.Create(new Homepage()
                    {
                        Title = "BANNER" + i.ToString(),
                        Type = PictureHomeType.BannerShop
                    });
                    _unitOfWork.SaveChanges();
                }
            }
        }
        public List<Homepage> GetBannerShopList()
        {
            CreateShopBannerIfNullOrNotOnly();
            return _homepageRepository.Filter(a => a.Type == PictureHomeType.SliderCategory || a.Type == PictureHomeType.BannerShop).ToList();
        }
        public void AddSliderShop(string imageId, string link)
        {
            _homepageRepository.Create(new Homepage()
            {
                Title = "SliderCategory",
                ImageId = imageId,
                Type = PictureHomeType.SliderCategory,
                Link = link
            });
            _unitOfWork.SaveChanges();
        }
        public void AddBannerShop(int Id, string imageId, string link)
        {
            var item = _homepageRepository.Find(Id);
            item.ImageId = imageId;
            item.Link = link ?? "#";
            _homepageRepository.Create(item);
            _unitOfWork.SaveChanges();
        }
        public void CreateSocialIfNullOrNotOnly()
        {
            string[] socialList = new string[3] { "FACEBOOK", "YOUTUBE", "INSTAGRAM" };
            foreach (var social in socialList)
            {
                var item = _homepageRepository.Filter(a => a.Type == PictureHomeType.Social && a.Title == social);
                if (item.Count() > 1 || item.Count() == 0)
                {
                    if (item.Count() > 1)
                        foreach (var j in item)
                            _homepageRepository.Delete(j);

                    _homepageRepository.Create(new Homepage()
                    {
                        Title = social,
                        Type = PictureHomeType.Social,
                        Link = "#"
                    });
                    _unitOfWork.SaveChanges();
                }
            }
        }
        public List<Homepage> GetSociallist()
        {
            CreateSocialIfNullOrNotOnly();
            return _homepageRepository.Filter(a => a.Type == PictureHomeType.Social).ToList();
        }
        public void UpdateSocial(string Facebook, string Youtube, string Instagram)
        {
            string[] socialList = new string[3] { "FACEBOOK", "YOUTUBE", "INSTAGRAM" };
            string[] link = new string[3] { Facebook, Youtube, Instagram };
            for (var i = 0; i <= 2; i++)
            {
                var item = _homepageRepository.Filter(a => a.Type == PictureHomeType.Social && a.Title == socialList[i]).FirstOrDefault();
                item.Link = link[i];
                _homepageRepository.Update(item);
            }
            _unitOfWork.SaveChanges();
        }

        //Question
        public IEnumerable<Question> GetQuestionList()
        {
            return _questionRepository.GetAll();
        }
        public Question GetQuestionById(int? id)
        {
            return _questionRepository.GetById(id);
        }
        public void CreateQuestion(Question question)
        {
            _questionRepository.Create(question);
            _unitOfWork.SaveChanges();
        }
        public void UpdateQuestion(Question question)
        {
            _questionRepository.Update(question);
            _unitOfWork.SaveChanges();
        }
        public void DeleteQuestion(int id)
        {
            var question = _questionRepository.GetById(id);
            _questionRepository.Delete(question);
            _unitOfWork.SaveChanges();
        }

        //Setup - Policy
        public Setting CreatePolicyIfNull()
        {
            var policy = new Setting()
            {
                SettingId = "Policy",
                Title = "Điều khoản sử dụng",
                Content = "<p></p>",
                Status = Status.Public
            };
            _settingRepository.Create(policy);
            _unitOfWork.SaveChanges();
            return policy;
        }
        public Setting GetPolicy()
        {
            var policy = _settingRepository.GetById("Policy");
            if (policy == null)
                policy = CreatePolicyIfNull();
            return policy;
        }
        public void UpdatePolicy(Setting policy)
        {
            var item = _settingRepository.Find("Policy");
            if (policy == null)
                item = CreatePolicyIfNull();
            item.Title = policy.Title;
            item.Content = policy.Content;
            item.Status = policy.Status;
            _settingRepository.Update(item);
            _unitOfWork.SaveChanges();
        }

        //Downpage - Bảo trì
        public Setting CreateDownpageIfNull()
        {
            var downpage = new Setting()
            {
                SettingId = "DownPage",
                IsDownPage = false
            };
            _settingRepository.Create(downpage);
            _unitOfWork.SaveChanges();
            return downpage;
        }

        public Setting GetDownpage()
        {
            var downpage = _settingRepository.GetById("DownPage");
            if (downpage == null)
                downpage = CreateDownpageIfNull();
            return downpage;
        }
        public void UpdateDownpage(Setting policy)
        {
            var item = _settingRepository.Find("Policy");
            if (policy == null)
                item = CreateDownpageIfNull();
            item.Title = policy.Title;
            item.Content = policy.Content;
            item.Status = policy.Status;
            _settingRepository.Update(item);
            _unitOfWork.SaveChanges();
        }

        //Top text
        public Setting CreateToptextIfNull()
        {
            var downpage = new Setting()
            {
                SettingId = "TOPTEXT",
                Content = "Kingflix - Tài khoản bản quyền Netflix, Spotify, ..."
            };
            _settingRepository.Create(downpage);
            _unitOfWork.SaveChanges();
            return downpage;
        }

        public Setting GetToptext()
        {
            var downpage = _settingRepository.GetById("TOPTEXT");
            if (downpage == null)
                downpage = CreateToptextIfNull();
            return downpage;
        }
        public void UpdateToptext(string content)
        {
            var item = _settingRepository.Find("TOPTEXT");
            if (item == null)
                item = CreateToptextIfNull();
            item.Content = content;
            _settingRepository.Update(item);
            _unitOfWork.SaveChanges();
        }

        //Maintain - Bao tri
        //Top text
        public Setting CreateBaoTriPageIfNull()
        {            
            var downpage = new Setting()
            {
                SettingId = "BaoTri",
                Title = "BẢO TRÌ HỆ THỐNG!",
                Content = "Xin lỗi quý khách vì sự bất tiện này, vui lòng quay lại sau ít phút.",
                Status = Status.Public
            };
            _settingRepository.Create(downpage);
            _unitOfWork.SaveChanges();
            return downpage;
        }

        public Setting GetBaoTriPage()
        {
            var downpage = _settingRepository.GetById("BaoTri");
            if (downpage == null)
                downpage = CreateBaoTriPageIfNull();
            return downpage;
        }
        public void UpdateBaoTriPage(Setting baotri)
        {
            var item = _settingRepository.Find("BaoTri");
            if (item == null)
                item = CreateBaoTriPageIfNull();
            item.Title = baotri.Title;
            item.Content = baotri.Content;
            item.Status = baotri.Status;
            item.Content = baotri.Content;
            _settingRepository.Update(item);
            _unitOfWork.SaveChanges();
        }
    }
}