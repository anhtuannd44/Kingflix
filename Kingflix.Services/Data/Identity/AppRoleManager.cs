using Kingflix.Domain.DomainModel.IdentityModel;
using Microsoft.AspNet.Identity;

namespace Kingflix.Services.Data.Identity
{
    public class AppRoleManager : RoleManager<AppRole>
    {
        public AppRoleManager(IRoleStore<AppRole, string> store) : base(store)
        {

        }
    }
}
