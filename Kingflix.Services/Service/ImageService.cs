using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using System;
using System.IO;

namespace Kingflix.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Image> _imageRepository;
        public ImageService(IUnitOfWork unitOfWork, IRepository<Image> imageRepository)
        {
            _unitOfWork = unitOfWork;
            _imageRepository = imageRepository;
        }
        public string CheckImageExist(string name, string ext)
        {
            var check = _imageRepository.Find(name + ext);
            while (check != null)
            {
                name += "-copy";
                var check2 = _imageRepository.Find(name + ext);
                if (check2 == null)
                    return name;
            };
            return name;
        }

        public void SaveImageInDb(string imageName)
        {
            var image = new Image()
            {
                ImageId = imageName,
                DateCreated = DateTime.Now
            };
            _imageRepository.Create(image);
            _unitOfWork.SaveChanges();
        }
        public int RemoveImageInDb(string imageName)
        {
            try
            {
                var image = _imageRepository.Find(imageName);
                _imageRepository.Delete(image);
                _unitOfWork.SaveChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public void RemoveImageInServer(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}