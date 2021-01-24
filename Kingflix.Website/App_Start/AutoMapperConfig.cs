using AutoMapper;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Website.Models;

namespace Kingflix.Website.App_Start
{
    public class AutoMapperConfig : AutoMapper.Profile
    {
        public static IMapper Register()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<AppUser, UserEditViewModel>();

                cfg.CreateMap<UserEditViewModel, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

                cfg.CreateMap<AppUser, UserCreateViewModel>();

                cfg.CreateMap<UserCreateViewModel, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }
    }
}