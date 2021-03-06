using Kingflix.Domain.ViewModel;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Kingflix.Services.Interfaces
{
    public interface IProductService
    {
        //Category - Loại sản phẩm
        IQueryable<Category> GetAllCategories();
        IQueryable<Category> GetAllCategories(TypeOfAccount type);
        Category GetCategoryById(string categoryId);
        string GenerateCategoryId();
        void CreateCategory(Category category, List<Price> prices);
        void UpdateCategory(Category category, List<Price> prices);
        void RemoveCategory(string categoryId);
        List<CategoryNetflixDetails> GetCategoryNetflixDetails(bool justPublic = true);
        List<CategoryNetflixDetails> GenerateCategoryNetflixDetails();
        void UpdateCategoryDetails(List<CategoryNetflixDetails> CategoryNetflixDetails);
        List<Category> GetNetflixList();
        List<Category> GetUpsaleList();
        Price GetCategoryPrice(string categoryId, double month);
        List<Price> GetPriceListByCategoryId(string categoryId);

        //Product - Tài khoản
        Product GetProductById(string productId);
        IEnumerable<Product> GetProductList(Expression<Func<Product, bool>> predicate = null);
        void CreateProductAndProfile(Product product, List<ProfileViewModel> Profile);
        ResultViewModel GetAndSaveProductDataFromExcel(DataSet result);
        void ChangeProductListStatus(string[] productList, ProductStatus status);
        void UpdateProduct(Product product);
        ResultViewModel DeleteProduct(string productId);
        ResultViewModel DeleteProductList(string[] productList);
        IEnumerable<Product> GetProductReplaceList(string categoryId, string currentProductId);
        ResultViewModel EditProductConfirm(Product product, string newPassword, bool changePassRequired, string currentParentId);

        //Profile
        IEnumerable<Profile> GetProfileList(Expression<Func<Profile, bool>> predicate = null);
        Profile GetProfileById(int id);
        void RemoveUserFromProfile(int[] profileList);
        Profile AddProfileToAttack(Profile profile);
        void EditProfileList(List<Profile> ProfileList);
        void CreateProfile(string productId);
        ResultViewModel DeleteProfile(int profileId);
        ResultViewModel ChangeProfile(int currentProfile, string product);
        void ResetProfile(int profileId);
    }
}