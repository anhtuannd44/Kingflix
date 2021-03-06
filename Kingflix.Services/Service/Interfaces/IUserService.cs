using Kingflix.Domain.DomainModel.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingflix.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<AppUser> GetUserList(Expression<Func<AppUser, bool>> predicate = null);
        AppUser GetUserById(string userId);
        void UpdateUser(AppUser user);
        void DeleteUser(string userId);
        void Refund(string userId, double amount);
    }
}