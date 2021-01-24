using System.Web.Mvc;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data.Identity.Abstraction;
using Microsoft.AspNet.Identity;
using NLog;

namespace Kingflix.Website.Controllers
{
    public class BaseExtendedController : BaseController
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IAppUserManager _userManager;

        public BaseExtendedController(
            IUnitOfWork unitOfWork,
            IAppUserManager userManager,
            IMapper mapper)
            : base(mapper)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [NonAction]
        protected AppUser GetCurrentUser()
        {
            string userId = ControllerContext.HttpContext.User.Identity.GetUserId();
            AppUser user = _userManager.FindById(userId);
            return user;
        }
    }
}