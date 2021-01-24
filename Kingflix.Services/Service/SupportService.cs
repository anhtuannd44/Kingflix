using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using System.Web;
using Kingflix.Utilities;
using System.Linq.Expressions;

namespace Kingflix.Services
{
    public class SupportService : ISupportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Support> _supportRepository;
        public SupportService(
            IUnitOfWork unitOfWork,
            IRepository<Support> supportRepository
           )
        {
            _unitOfWork = unitOfWork;
            _supportRepository = supportRepository;
        }
        public IEnumerable<Support> GetSupportList(Expression<Func<Support, bool>> predicate = null)
        {
            return _supportRepository.Filter(predicate);
        }
        public Support GetSupportById(int? id)
        {
            return _supportRepository.GetById(id);
        }
        public void UpdateSupport(Support item)
        {
            _supportRepository.Update(item);
            _unitOfWork.SaveChanges();
        }
    }
}