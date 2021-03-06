using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Kingflix.Services.Interfaces
{
    public interface ISupportService
    {
        IEnumerable<Support> GetSupportList(Expression<Func<Support, bool>> predicate = null);
        void CreateSupport(Support item);
        Support GetSupportById(int? id);
        void UpdateSupport(Support item);
    }
}