using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using System.Collections;
using System.Collections.Generic;

namespace Kingflix.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Review> _reviewRepository;
        public ReviewService(
             IUnitOfWork unitOfWork,
             IRepository<Review> reviewRepository
             )
        {
            _unitOfWork = unitOfWork;
            _reviewRepository = reviewRepository;
        }
        public IEnumerable<Review> GetReviewList()
        {
            return _reviewRepository.GetAll();
        }
        public Review GetReviewById(int? id)
        {
            return _reviewRepository.GetById(id);
        }
        public void UpdateReview(Review review)
        {
            _reviewRepository.Update(review);
            _unitOfWork.SaveChanges();
        }
        public void DeleteReview(int id)
        {
            var item = _reviewRepository.GetById(id);
            _reviewRepository.Delete(item);
            _unitOfWork.SaveChanges();
        }
    }
}