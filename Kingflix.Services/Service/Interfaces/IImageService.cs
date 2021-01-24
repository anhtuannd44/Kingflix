using Kingflix.Domain.ViewModel;
using System.Threading.Tasks;

namespace Kingflix.Services.Interfaces
{
    public interface IImageService
    {
        string CheckImageExist(string name, string ext);
        void SaveImageInDb(string imageName);
        int RemoveImageInDb(string imageName);
        void RemoveImageInServer(string path);
    }
}