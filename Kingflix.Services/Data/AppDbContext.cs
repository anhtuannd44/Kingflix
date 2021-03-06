using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data.EntitiesConfigurations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kingflix.Services.Data
{
    public class AppDbContextInit : NullDatabaseInitializer<AppDbContext>
    {

    }

    public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
    {
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryNetflixDetails> CategoryNetflixDetails { get; set; }
        public virtual DbSet<Price> Price { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Voucher> Voucher { get; set; }
        public virtual DbSet<VoucherChild> VoucherChild { get; set; }
        public virtual DbSet<Voucher_Category> Voucher_Categories { get; set; }
        public virtual DbSet<Support> Supports { get; set; }
        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<BlogCategory> BlogCategory { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<KingCoin> KingCoin { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailHistory> EmailHistory { get; set; }
        public virtual DbSet<ExtendProfile> ExtendProfiles { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Homepage> Homepages { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<FlashSale> FlashSale { get; set; }
        public virtual DbSet<FlashSaleCategory> FlashSaleCategories { get; set; }
        public virtual DbSet<GoogleForm> GoogleForm { get; set; }
        public virtual DbSet<CardTemplate> CardTemplates { get; set; }
        public virtual DbSet<SMSHistory> SMSHistory { get; set; }
        public virtual DbSet<SMSTemplate> SMSTemplates { get; set; }
        public virtual DbSet<RefundSetting> RefundSetting { get; set; }

        public AppDbContext() : base("KingflixDb")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Configurations.Add(new UserEntityConfig());

            modelBuilder.Entity<AppUser>()
                        .HasMany(c => c.SMSHistory)
                        .WithOptional(c => c.UserInformation)
                        .HasForeignKey(c => c.UserId)
                        .WillCascadeOnDelete(false);
        }
    }
}
