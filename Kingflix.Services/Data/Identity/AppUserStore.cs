using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kingflix.Services.Data.Identity
{
    public class AppUserStore : UserStore<AppUser>
    {
        public AppUserStore(IAppDbContext context) : base(context as AppDbContext)
        {
        }
    }
}
