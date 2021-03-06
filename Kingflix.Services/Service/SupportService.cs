using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using System.Collections.Generic;
using System;
using Kingflix.Domain.Abstract;
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
        public void CreateSupport(Support item)
        {
            _supportRepository.Create(item);
            _unitOfWork.SaveChanges();
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