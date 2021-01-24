using Kingflix.Services.Interfaces;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace Kingflix.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AppUser> _userRepository;
        public UserService(
             IUnitOfWork unitOfWork,
             IRepository<AppUser> userRepository
             )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }
        public IEnumerable<AppUser> GetUserList(Expression<Func<AppUser, bool>> predicate = null)
        {
            return _userRepository.Filter(predicate);
        }
        public AppUser GetUserById(string userId)
        {
            return _userRepository.GetById(userId);
        }
        public void UpdateUser(AppUser AppUser)
        {
            var user = _userRepository.GetById(AppUser.Id);
            user.FullName = AppUser.FullName;
            user.Address = AppUser.Address;
            user.Birthday = AppUser.Birthday;
            user.Gentle = AppUser.Gentle;
            user.PhoneNumber = AppUser.PhoneNumber;
            _userRepository.Update(user);
            _unitOfWork.SaveChanges();
        }
        public void DeleteUser(string userId)
        {
            AppUser AppUser = _userRepository.GetById(userId);
            _userRepository.Delete(AppUser);
            _unitOfWork.SaveChanges();
        }
    }
}