using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Kingflix.Services.Interfaces
{
    public interface IReviewService
    {
        IEnumerable<Review> GetReviewList();
        Review GetReviewById(int? id);
        void UpdateReview(Review review);
        void DeleteReview(int id);
    }
}