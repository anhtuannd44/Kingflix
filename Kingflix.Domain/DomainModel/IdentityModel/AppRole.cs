using Microsoft.AspNet.Identity.EntityFramework;

namespace Kingflix.Domain.DomainModel.IdentityModel
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }
    }
}
