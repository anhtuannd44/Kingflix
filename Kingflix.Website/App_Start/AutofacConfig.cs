using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services;
using Kingflix.Services.Data;
using Kingflix.Services.Data.Identity;
using Kingflix.Services.Data.Identity.Abstraction;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;

[assembly: OwinStartup(typeof(Kingflix.Website.App_Start.AutofacConfig))]

namespace Kingflix.Website.App_Start
{
    public partial class AutofacConfig
    {
        public void Configuration(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // AutoMapper config
            builder.Register(c => AutoMapperConfig.Register()).As<IMapper>().InstancePerLifetimeScope().PropertiesAutowired().PreserveExistingDefaults();

            // DbContext config
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<AppDbContext>().As<IAppDbContext>().InstancePerRequest();
            builder.RegisterType<AppDbContext>().AsSelf().InstancePerRequest();

            // Identity config
            builder.RegisterType<AppSignInManager>().AsSelf().InstancePerRequest();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).InstancePerRequest();
            builder.RegisterType<AppUserManager>().As<IAppUserManager>().InstancePerRequest();
            builder.RegisterType<AppRoleManager>().AsSelf().InstancePerRequest();
            builder.Register(c => new RoleStore<AppRole>(c.Resolve<AppDbContext>())).As<IRoleStore<AppRole, string>>().InstancePerRequest();
            builder.Register(c => new AppUserStore(c.Resolve<IAppDbContext>())).As<IUserStore<AppUser>>().InstancePerRequest();

            // Types config
            builder.RegisterGeneric(typeof(Repository<>)).As((typeof(IRepository<>))).InstancePerRequest();
            builder.RegisterType<MailingService>().As<IMailingService>().InstancePerRequest();
            builder.RegisterType<APIService>().As<IAPIService>().InstancePerRequest();
            builder.RegisterType<BlogService>().As<IBlogService>().InstancePerRequest();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerRequest();
            builder.RegisterType<ImageService>().As<IImageService>().InstancePerRequest();
            builder.RegisterType<KingCoinService>().As<IKingCoinService>().InstancePerRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerRequest();
            builder.RegisterType<PageService>().As<IPageService>().InstancePerRequest();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerRequest();
            builder.RegisterType<ReviewService>().As<IReviewService>().InstancePerRequest();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerRequest();
            builder.RegisterType<SupportService>().As<ISupportService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();

            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            IContainer container = builder.Build();

            // Set Dependency Resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            ConfigureAuth(app);
        }
    }
}