using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System.Collections.Generic;

namespace Kingflix.Services.Interfaces
{
    public interface IPageService
    {
        IEnumerable<Page> GetPageList();
        void CreatePage(Page page);
        Page GetPageById(int? id);
        void UpdatePage(Page page);
        void DeletePage(int id);
    }
}