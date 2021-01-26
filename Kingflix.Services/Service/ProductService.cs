using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Kingflix.Services
{
    public class ProductService : Controller, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<CategoryNetflixDetails> _categoryNetflixDetailsRepository;
        private readonly IRepository<Price> _priceRepository;
        private readonly IEmailService _emailService;
        public ProductService(
            IUnitOfWork unitOfWork,
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<Profile> profileRepository,
            IRepository<CategoryNetflixDetails> categoryNetflixDetailsRepository,
            IRepository<Price> priceRepository,
            IEmailService emailService
            )
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _profileRepository = profileRepository;
            _categoryNetflixDetailsRepository = categoryNetflixDetailsRepository;
            _priceRepository = priceRepository;
            _emailService = emailService;
        }
        public Category GetCategoryById(string categoryId)
        {
            return _categoryRepository.GetById(categoryId);
        }
        public IQueryable<Category> GetAllCategories()
        {
            return _categoryRepository.Get();
        }
        public IQueryable<Category> GetAllCategories(TypeOfAccount type)
        {
            return _categoryRepository.Filter(a => a.TypeOfAccount == type);
        }
        public string GenerateCategoryId()
        {
            var categoryId = HelperFunction.RandomString(15);
            var checkCategoryId = _categoryRepository.Find(categoryId);
            while (checkCategoryId != null)
            {
                categoryId = HelperFunction.RandomString(15);
                checkCategoryId = _categoryRepository.Find(categoryId);
            }
            return categoryId;
        }
        public void CreateCategory(Category category, List<Price> prices)
        {
            _categoryRepository.Create(category);
            _priceRepository.CreateRange(prices);
            _unitOfWork.SaveChanges();
        }
        public void UpdateCategory(Category category, List<Price> prices)
        {
            _categoryRepository.Update(category);
            _priceRepository.Delete(a => a.CategoryId == category.CategoryId);
            _priceRepository.CreateRange(prices);
            _unitOfWork.SaveChanges();
        }
        public void RemoveCategory(string categoryId)
        {
            var item = _categoryRepository.GetById(categoryId);
            _categoryRepository.Delete(item);
            _unitOfWork.SaveChanges();
        }
        public List<CategoryNetflixDetails> GetCategoryNetflixDetails(bool justPublic = true)
        {
            var result = _categoryNetflixDetailsRepository.Get();
            if (justPublic)
                result = result.Where(a => a.IsShow);
            return result.OrderBy(a => a.OrderBy).ToList();
        }
        public List<CategoryNetflixDetails> GenerateCategoryNetflixDetails()
        {
            var list = new List<CategoryNetflixDetails>()
                {
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.NoChangeAccount,
                        CheckNetflix1 = false,
                        CheckNetflix2 = false,
                        CheckNetflix3 = true,
                        CheckNetflix4 = true,
                        IsShow = true,
                        OrderBy = 1,
                        Name = CategoryDetailsType.NoChangeAccount.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.Support,
                        ContentNetflix1 = "7 ngày đầu",
                        ContentNetflix2 = "True",
                        ContentNetflix3 = "True",
                        ContentNetflix4 = "True",
                        IsShow = true,
                        OrderBy = 2,
                        Name = CategoryDetailsType.Support.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.TimeAcceptOrder,
                        ContentNetflix1 = "1h - 3h",
                        ContentNetflix2 = "15p - 1h",
                        ContentNetflix3 = "15p - 1h",
                        ContentNetflix4 = "15p - 1h",
                        IsShow = true,
                        OrderBy = 3,
                        Name = CategoryDetailsType.TimeAcceptOrder.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.OwnProfile,
                        CheckNetflix1 = false,
                        CheckNetflix2 = false,
                        CheckNetflix3 = true,
                        CheckNetflix4 = true,
                        IsShow = true,
                        OrderBy = 4,
                        Name = CategoryDetailsType.OwnProfile.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.ScreenCount,
                        ContentNetflix1 = "1",
                        ContentNetflix2 = "1",
                        ContentNetflix3 = "1",
                        ContentNetflix4 = "4",
                        IsShow = true,
                        OrderBy =5,
                        Name = CategoryDetailsType.ScreenCount.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.UltraHD,
                        CheckNetflix1 = true,
                        CheckNetflix2 = true,
                        CheckNetflix3 = true,
                        CheckNetflix4 = true,
                        IsShow = true,
                        OrderBy = 6,
                        Name = CategoryDetailsType.UltraHD.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.MultiScreen,
                        CheckNetflix1 = true,
                        CheckNetflix2 = true,
                        CheckNetflix3 = true,
                        CheckNetflix4 = true,
                        IsShow = true,
                        OrderBy = 7,
                        Name = CategoryDetailsType.MultiScreen.GetEnumDisplayName()
                    },
                    new CategoryNetflixDetails()
                    {
                        Type = CategoryDetailsType.Download,
                        CheckNetflix1 = true,
                        CheckNetflix2 = true,
                        CheckNetflix3 = true,
                        CheckNetflix4 = true,
                        IsShow = true,
                        OrderBy = 8,
                        Name = CategoryDetailsType.Download.GetEnumDisplayName()
                    }
                };
            _categoryNetflixDetailsRepository.CreateRange(list);
            _unitOfWork.SaveChanges();
            return list;
        }
        public void UpdateCategoryDetails(List<CategoryNetflixDetails> CategoryNetflixDetails)
        {
            foreach (var item in CategoryNetflixDetails)
            {
                _categoryNetflixDetailsRepository.Update(item);
            }
            _unitOfWork.SaveChanges();
        }
        public List<Category> GetNetflixList()
        {
            return _categoryRepository.Filter(a => a.Status == Status.Public && a.Type == TypeOfCategory.Netflix).ToList();
        }
        public List<Category> GetUpsaleList()
        {
            return _categoryRepository.Filter(a => a.Status == Status.Public && a.Type == TypeOfCategory.Upsale).ToList();
        }
        public Price GetCategoryPrice(string categoryId, double month)
        {
            return _priceRepository.Find(categoryId, month);
        }

        //Product - Tài khoản
        public Product GetProductById(string productId)
        {
            return _productRepository.GetById(productId);
        }
        public IEnumerable<Product> GetProductList(Expression<Func<Product, bool>> predicate = null)
        {
            return _productRepository.Filter(predicate);
        }
        public void CreateProductAndProfile(Product product, List<ProfileViewModel> Profile)
        {
            product.DateCreated = DateTime.Now;
            _productRepository.Create(product);
            if (product.CategoryId == Const.NETFLIX3)
                _profileRepository.Create(new Profile()
                {
                    Name = "ProfileFamily",
                    Pincode = "1111",
                    DateCreated = DateTime.Now,
                    DateEnd = DateTime.Now,
                    DateStart = DateTime.Now,
                    ProductId = product.ProductId
                });
            else
                foreach (var item in Profile)
                    if (!string.IsNullOrEmpty(item.Name))
                        _profileRepository.Create(new Profile()
                        {
                            Name = item.Name,
                            Pincode = item.Pincode,
                            DateCreated = DateTime.Now,
                            ProductId = product.ProductId,
                            DateStart = DateTime.Now,
                            DateEnd = DateTime.Now
                        });
            _unitOfWork.SaveChanges();
        }
        public ResultViewModel GetAndSaveProductDataFromExcel(DataSet result)
        {
            var successCount = 0;
            if (result.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    try
                    {
                        _productRepository.Create(new Product()
                        {
                            ProductId = row[0].ToString(),
                            Password = row[1].ToString(),
                            CategoryId = row[2].ToString(),
                            Cycle = row[3].ToString() == "King" ? Cycle.King : row[3].ToString() == "Avenger" ? Cycle.Avenger : row[3].ToString() == "Mouse" ? Cycle.Mouse : Cycle.Mouse,
                            DateEnd = row[4].ToString().AsDateTime(),
                            DateCreated = DateTime.Now,
                            Status = ProductStatus.Private
                        });
                        for (int i = 5; i <= 23; i += 2)
                        {
                            if (!(string.IsNullOrEmpty(row[i].ToString()) && string.IsNullOrEmpty(row[i + 1].ToString())))
                            {
                                _profileRepository.Create(new Profile
                                {
                                    Name = row[i].ToString() ?? "Profile",
                                    Pincode = row[i + 1].ToString(),
                                    DateCreated = DateTime.Now,
                                    DateStart = DateTime.Now,
                                    DateEnd = DateTime.Now,
                                    ProductId = row[0].ToString()
                                });
                            }
                        }
                        _unitOfWork.SaveChanges();
                        successCount++;

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return new ResultViewModel()
            {
                status = "success",
                message = successCount + " tài khoản đã được thêm"
            };
        }
        public void ChangeProductListStatus(string[] productList, ProductStatus status)
        {
            foreach (var item in productList)
            {
                var product = _productRepository.Find(item);
                if (product != null)
                {
                    product.Status = status;
                    _productRepository.Update(product);
                }
            }
            _unitOfWork.SaveChanges();
        }
        public void UpdateProduct(Product product)
        {
            var productItem = _productRepository.GetById(product.ProductId);
            if (productItem.CategoryId != product.CategoryId)
            {
                var isNoUser = true;
                foreach (var i in productItem.Profiles)
                {
                    if (!string.IsNullOrEmpty(i.UserId))
                    {
                        isNoUser = false;
                        break;
                    }
                }
                if (isNoUser)
                    productItem.CategoryId = product.CategoryId;
            }

            productItem.DateModified = DateTime.Now;
            productItem.Password = product.Password;
            productItem.DateEnd = product.DateEnd;
            productItem.Status = product.Status;
            _productRepository.Update(productItem);
            _unitOfWork.SaveChanges();
        }
        public ResultViewModel DeleteProduct(string productId)
        {
            var result = new ResultViewModel();
            var product = _productRepository.Find(productId);
            var isUserInProfile = false;
            foreach (var item in product.Profiles.ToList())
            {
                if (!string.IsNullOrEmpty(item.UserId))
                {
                    isUserInProfile = true;
                    break;
                }
            }
            if (isUserInProfile)
            {
                result.status = "error";
                result.message = "Thất bại! Tài khoản đang có người sử dụng";
            }
            else
            {
                _productRepository.Delete(product);
                _unitOfWork.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Tài khoản đã được xóa";
            }
            return result;
        }
        public IEnumerable<Product> GetProductReplaceList(string categoryId, string currentProductId)
        {
            return _productRepository.Filter(a => a.CategoryId == categoryId && a.ProductId != currentProductId);
        }
        public ResultViewModel EditProductConfirm(Product product, string newPassword, bool changePassRequired, string currentParentId)
        {
            var result = new ResultViewModel();
            var productItem = _productRepository.GetById(product.ProductId);
            if (productItem.CategoryId != product.CategoryId)
            {
                var isNoUser = true;
                foreach (var i in productItem.Profiles)
                {
                    if (!string.IsNullOrEmpty(i.UserId))
                    {
                        isNoUser = false;
                        break;
                    }
                }
                if (isNoUser)
                    productItem.CategoryId = product.CategoryId;
                else
                {
                    result.status = "error";
                    result.message = "Thất bại! Không thể chuyển Tài khoản vì đang có người sử dụng";
                    return result;
                }
            }

            productItem.DateModified = DateTime.Now;
            productItem.Password = product.Password;
            productItem.DateEnd = product.DateEnd;
            productItem.Status = product.Status;

            if (product.Status != ProductStatus.Replace)
                product.ParentId = null;

            _productRepository.Update(productItem);

            if (changePassRequired)
            {
                var parent = _productRepository.GetById(currentParentId);
                parent.Password = newPassword;
                _productRepository.Update(parent);
            }

            foreach (var item in product.ProfileList)
            {
                if (string.IsNullOrEmpty(item.Name))
                    item.Pincode = null;

                var editItem = _profileRepository.GetById(item.ProfileId);
                editItem.Name = item.Name;
                editItem.Pincode = item.Pincode;
                editItem.DateModified = DateTime.Now;
                _profileRepository.Update(editItem);
            }
            _unitOfWork.SaveChanges();
            result.status = "success";
            result.message = "Thành công! Tài khoản đã được chỉnh sửa";
            return result;
        }

        //Profile
        public IEnumerable<Profile> GetProfileList(Expression<Func<Profile, bool>> predicate = null)
        {
            return _profileRepository.Filter(predicate);
        }
        public void RemoveUserFromProfile(int[] profileList)
        {
            foreach (var item in profileList)
            {
                var profile = _profileRepository.GetById(item);
                profile.UserId = null;
                _profileRepository.Update(profile);
            }
            _unitOfWork.SaveChanges();
        }
        public ResultViewModel DeleteProductList(string[] productList)
        {
            var result = new ResultViewModel();
            //Chuyển Profile cho người dùng còn hạn
            var allPofileList = _profileRepository.GetAll();
            var profileList = new List<Profile>(); //Danh sách profile có người dùng còn hạn mà tài khoản hết hạn
            foreach (var item in productList)
            {
                var profileListExpried = allPofileList.Where(a => a.ProductId == item && a.DateEnd > DateTime.Today && a.Products.DateEnd <= DateTime.Today);
                if (profileListExpried.Count() != 0)
                    profileList.AddRange(profileListExpried);
            }
            var profileAvailable = allPofileList.Where(a => string.IsNullOrEmpty(a.UserId) && a.Products.DateEnd > DateTime.Today).ToList(); //Danh sách Profile còn hạn và đang trống
            if (profileList.Count > 0) //Nếu tồn tại người dùng còn hạn mà tài khoản hết hạn
            {
                foreach (var item in profileList)
                {
                    //Lấy danh sách những profile đang trống và còn hạn
                    var editItemList = profileAvailable.Where(a => a.Products.CategoryId == item.Products.CategoryId).ToList();
                    if (item.Products.CategoryId == Const.NETFLIX2)
                        editItemList = editItemList.Where(a => a.Products.Cycle == item.Products.Cycle).ToList();
                    if (editItemList.Count > 0)
                    {
                        //Thêm user vào Profile mới
                        var editItem = editItemList.FirstOrDefault();
                        profileAvailable.Where(a => a.ProfileId == editItem.ProfileId).FirstOrDefault().UserId = item.UserId;
                        profileAvailable.Where(a => a.ProfileId == editItem.ProfileId).FirstOrDefault().DateStart = item.DateStart;
                        profileAvailable.Where(a => a.ProfileId == editItem.ProfileId).FirstOrDefault().DateEnd = item.DateEnd;
                        _profileRepository.Update(profileAvailable.Where(a => a.ProfileId == editItem.ProfileId).FirstOrDefault());

                        //Xóa User khỏi profile cũ
                        item.UserId = null;
                        _profileRepository.Update(item);
                    }
                    else
                    {
                        result.status = "error";
                        result.message = "Số Profile trống còn lại không đủ. Vui lòng tạo thêm tài khoản hoặc Profile";
                        return result;
                    }
                }
            }

            //Xóa tài khoản không còn người dùng còn hạn
            var products = profileList.Where(a => a.DateEnd > DateTime.Today).Select(a => a.Products).Distinct().ToList();
            foreach (var item in products)
            {
                _productRepository.Delete(item);
            }
            _unitOfWork.SaveChanges();
            result.status = "success";
            result.message = "Đã xóa tài khoản hết hạn thành công!";
            return result;
        }
        public Profile GetProfileById(int id)
        {
            return _profileRepository.GetById(id);
        }
        public Profile AddProfileToAttack(Profile profile)
        {
            _profileRepository.Update(profile);
            return profile;
        }
        public void EditProfileList(List<Profile> ProfileList)
        {
            foreach (var item in ProfileList)
            {
                var editItem = _profileRepository.GetById(item.ProfileId);
                var currentUserId = editItem.UserId;
                if (editItem != null)
                {
                    editItem.Name = item.Name;
                    editItem.Pincode = item.Pincode;
                    if (string.IsNullOrEmpty(item.UserId))
                    {
                        editItem.DateStart = DateTime.Now;
                        editItem.DateEnd = DateTime.Now;
                    }
                    else
                    {
                        editItem.UserId = item.UserId;
                        editItem.DateStart = item.DateStart;
                        editItem.DateEnd = item.DateEnd;
                    }
                }
                _profileRepository.Update(editItem);

                if (currentUserId != editItem.UserId && editItem.UserId != null)
                {
                    var emailBody = ViewToStringRenderer.RenderViewToString(this.ControllerContext, "~/Views/Emails/ChangeProfile.cshtml", editItem);
                    var sendEmail = _emailService.SendEmailCustom(editItem.UserInformation.Email, new EmailTemplate() { Title = "Kingflix - Thay đổi tài khoản", Content = emailBody });
                    if (sendEmail)
                        _emailService.AddEmailHistory(EmailTypeHistory.AccountRegister, editItem.UserInformation.Email);
                }
                _unitOfWork.SaveChanges();
            }
        }
        public void CreateProfile(string productId)
        {
            var profile = new Profile()
            {
                ProductId = productId,
                DateCreated = DateTime.Now,
                Name = "Profile",
                Pincode = "trống",
                DateEnd = DateTime.Now,
                DateStart = DateTime.Now,
            };
            _profileRepository.Update(profile);
            _unitOfWork.SaveChanges();
        }
        public ResultViewModel DeleteProfile(int profileId)
        {
            var result = new ResultViewModel();
            var profile = _profileRepository.GetById(profileId);
            var profileList = _profileRepository.Filter(a => a.ProductId == profile.ProductId && a.ProductId != null);
            if (profileList.Count() > 5)
            {
                if (string.IsNullOrEmpty(profile.UserId))
                {
                    _profileRepository.Delete(profile);
                    _unitOfWork.SaveChanges();
                    result.status = "success";
                    result.message = "Thành công! Đã xóa Profile khỏi tài khoản";
                }
                else
                {
                    result.status = "error";
                    result.message = "Thất bại! Profile đang có người sử dụng!";
                }
            }
            else
            {
                result.status = "error";
                result.message = "Thất bại! Số lượng Profile của tài khoản phải từ 5 trở lên!";
            }
            return result;
        }
        public ResultViewModel ChangeProfile(int currentProfile, string product)
        {
            var result = new ResultViewModel();
            var item = _profileRepository.GetById(currentProfile);
            var newItem = _profileRepository.Get().Where(a => a.ProductId == product && a.ProductId != item.ProductId && a.Products.DateEnd.Date >= DateTime.Today && string.IsNullOrEmpty(a.UserId)).FirstOrDefault();
            newItem.UserId = item.UserId;
            newItem.DateEnd = item.DateEnd;
            newItem.DateModified = DateTime.Now;
            newItem.DateStart = DateTime.Now;

            _profileRepository.Update(newItem);
            _unitOfWork.SaveChanges();

            var emailBody = ViewToStringRenderer.RenderViewToString(this.ControllerContext, "~/Views/Emails/ChangeProfile.cshtml", newItem);
            var sendEmail = _emailService.SendEmailCustom(item.UserInformation.Email, new EmailTemplate() { Title = "Kingfix - Thông báo tài khoản NETFLIX mới", Content = emailBody });

            if (sendEmail)
            {
                _emailService.AddEmailHistory(EmailTypeHistory.ChangeProfile, item.UserInformation.Email);
                _unitOfWork.SaveChanges();
            }
            result.status = "success";
            result.message = "Thành công! Người dùng đã được đổi tài khoản";
            return result;
        }
        public void ResetProfile(int profileId)
        {
            var item = _profileRepository.GetById(profileId);
            item.UserId = null;
            _profileRepository.Update(item);
            _unitOfWork.SaveChanges();
        }
    }
}