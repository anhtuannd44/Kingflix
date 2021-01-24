using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingflix.Services
{
    public class PageService : IPageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Page> _pageRepository;
        public PageService(
            IUnitOfWork unitOfWork,
            IRepository<Page> pageRepository
            )
        {
            _unitOfWork = unitOfWork;
            _pageRepository = pageRepository;
        }
        public IEnumerable<Page> GetPageList()
        {
            return _pageRepository.GetAll();
        }
        public void CreatePage(Page page)
        {
            page.DateCreated = DateTime.Now;
            page.Url = CheckUrl(page.Title, page.Url);
            _pageRepository.Create(page);
            _unitOfWork.SaveChanges();
        }
        public Page GetPageById(int? id)
        {
            return _pageRepository.GetById(id);
        }
        public void UpdatePage(Page page)
        {
            page.DateModified = DateTime.Now;
            page.Url = CheckUrl(page.Title, page.Url);
            _pageRepository.Update(page);
            _unitOfWork.SaveChanges();
        }
        public void DeletePage(int id)
        {
            var page = _pageRepository.GetById(id);
            _pageRepository.Delete(page);
            _unitOfWork.SaveChanges();
        }

        public string CheckUrl(string name, string url)
        {
            try
            {
                var newUrl = string.Empty;
                if (string.IsNullOrEmpty(url))
                    newUrl = HelperFunction.CreateUrl(name);
                else
                    newUrl = HelperFunction.CreateUrl(url);

                var checkUrl = false;
                var i = 0;
                while (!checkUrl)
                {
                    var productCheck = _pageRepository.Filter(a => a.Url == newUrl).ToList();
                    if (productCheck.Count == 0)
                        checkUrl = true;
                    else
                        newUrl += "-" + i;
                    i++;
                }
                return newUrl;
            }
            catch
            {
                return null;
            }
        }
    }
}