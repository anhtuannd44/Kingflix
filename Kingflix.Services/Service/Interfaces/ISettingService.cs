using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Kingflix.Services.Interfaces
{
    public interface ISettingService 
    {
        List<Homepage> GetListHomepageSlider();
        ResultViewModel AddSlider(string ImageId, string Title, string Content, string subContent, string YoutubeUrl, string contentBackground, string titleBackground, string subContentBackground, string TextFixed);
        ResultViewModel EditSider(int id, string title, string content, string imageId, string subcontent, string youtubeUrl, string contentBackground, string titleBackground, string subContentBackground, string textFixed);
        ResultViewModel DeleteSlider(int id);
        List<Homepage> GetSmallPicture();
        ResultViewModel EditSmallPicture(int id, string title, string content, string imageId, string backgroundColor);
        void CreateShopBannerIfNullOrNotOnly();
        List<Homepage> GetBannerShopList();
        void AddSliderShop(string imageId, string link);
        void AddBannerShop(int Id, string imageId, string link);
        void CreateSocialIfNullOrNotOnly();
        List<Homepage> GetSociallist();
        void UpdateSocial(string Youtube,string Facebook,string Instagram);
        IEnumerable<Question> GetQuestionList();
        Question GetQuestionById(int? id);
        void CreateQuestion(Question question);
        void UpdateQuestion(Question question);
        void DeleteQuestion(int id);

        //Setup
        Setting CreatePolicyIfNull();
        Setting GetPolicy();
        void UpdatePolicy(Setting policy);

        //Downpage
        Setting CreateDownpageIfNull();
        Setting GetDownpage();
        void UpdateDownpage(Setting downpage);

        //TOP text
        Setting CreateToptextIfNull();
        Setting GetToptext();
        void UpdateToptext(string content);

        //Maintain - Bảo trì
        Setting CreateBaoTriPageIfNull();
        Setting GetBaoTriPage();
        void UpdateBaoTriPage(Setting baotri);
    }
}