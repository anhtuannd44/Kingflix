using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System.Collections.Generic;

namespace Kingflix.Services.Interfaces
{
    public interface IBlogService
    {
        IEnumerable<Blog> GetBlogList();
        Blog GetBlogById(int? id);
        void CreateBlog(Blog blog);
        void UpdateBlog(Blog blog);
        void DeleteBlog(int id);
        IEnumerable<BlogCategory> GetBlogCategoryList();
        BlogCategory GetBlogCategoryById(int? id);
        void CreateBlogCategory(string blogCategoryName, BlogType type);
        void UpdateBlogCategory(int BlogCategoryId, string BlogCategoryNameEdit);
        void DeleteBlogCategory(int id);

        //Recruiment
        IEnumerable<Blog> GetRecruimentList();
        IEnumerable<BlogCategory> GetRecruimentCategoryList();
    }
}