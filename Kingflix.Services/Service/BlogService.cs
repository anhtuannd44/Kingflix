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
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<BlogCategory> _blogCategoryRepository;
        public BlogService(
            IUnitOfWork unitOfWork,
            IRepository<Blog> blogRepository,
            IRepository<BlogCategory> blogCategoryRepository
            )
        {
            _unitOfWork = unitOfWork;
            _blogRepository = blogRepository;
            _blogCategoryRepository = blogCategoryRepository;
        }
        public IEnumerable<Blog> GetBlogList()
        {
            return _blogRepository.Filter(a => a.BlogCategory.Type == BlogType.Blog);
        }
        public Blog GetBlogById(int? id)
        {
            return _blogRepository.GetById(id);
        }
        public void CreateBlog(Blog blog)
        {

            blog.DateCreated = DateTime.Now;
            blog.Url = CheckUrl(blog.Title, blog.Url);
            _blogRepository.Create(blog);
            _unitOfWork.SaveChanges();
        }
        public void UpdateBlog(Blog blog)
        {
            blog.DateModified = DateTime.Now;
            blog.Url = CheckUrl(blog.Title, blog.Url);
            _blogRepository.Update(blog);
            _unitOfWork.SaveChanges();
        }
        public void DeleteBlog(int id)
        { 
            var blog = _blogRepository.Find(id);
            _blogRepository.Delete(blog);
            _unitOfWork.SaveChanges();
        }
        public IEnumerable<BlogCategory> GetBlogCategoryList()
        {
            return _blogCategoryRepository.GetAll().Where(a=>a.Type == BlogType.Blog);
        }
        public BlogCategory GetBlogCategoryById(int? id)
        {
            return _blogCategoryRepository.GetById(id);
        }
        public void CreateBlogCategory(string blogCategoryName, BlogType type)
        {
            var item = new BlogCategory()
            {
                Name = blogCategoryName,
                DateCreated = DateTime.Now,
                Url = CheckUrl(blogCategoryName, null),
                Type = type
            };
            _blogCategoryRepository.Create(item);
            _unitOfWork.SaveChanges();
        }
        public void UpdateBlogCategory(int BlogCategoryId, string BlogCategoryNameEdit)
        {
            var item = _blogCategoryRepository.GetById(BlogCategoryId);
            item.Name = BlogCategoryNameEdit;
            item.Url = CheckUrl(BlogCategoryNameEdit, null);
            _blogCategoryRepository.Update(item);
            _unitOfWork.SaveChanges();
        }
        public void DeleteBlogCategory(int id)
        {
            var blogCategory = _blogCategoryRepository.Find(id);
            _blogCategoryRepository.Delete(blogCategory);
            _unitOfWork.SaveChanges();
        }

        //Recruiment
        public IEnumerable<Blog> GetRecruimentList()
        {
            return _blogRepository.Filter(a => a.BlogCategory.Type == BlogType.Recruitment);
        }

        public IEnumerable<BlogCategory> GetRecruimentCategoryList()
        {
            return _blogCategoryRepository.GetAll().Where(a => a.Type == BlogType.Recruitment);
        }


        //CheckURL
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
                    var productCheck = _blogRepository.Filter(a => a.Url == newUrl);
                    if (productCheck.Count() == 0)
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