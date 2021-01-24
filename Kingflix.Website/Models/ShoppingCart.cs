using Kingflix.Domain.Enumerables;
using Kingflix.Models.ViewModel;
using Kingflix.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kingflix.Models
{
    public class ShoppingCart
    {

        // Lấy giỏ hàng từ Session
        public static ShoppingCart Cart
        {
            get
            {
                var cart = HttpContext.Current.Session["Cart"] as ShoppingCart;
                // Nếu chưa có giỏ hàng trong session -> tạo mới và lưu vào session
                if (cart == null)
                {
                    cart = new ShoppingCart();
                    HttpContext.Current.Session["Cart"] = cart;
                }
                return cart;
            }
        }

        // Chứa các mặt hàng đã chọn

        public List<CartViewModel> Items = new List<CartViewModel>();

        public void Add(string categoryId, double Month, int count, TypeOfAccount type, string user, string pass)
        {
            try // tìm thấy trong giỏ -> tăng số lượng lên 1
            {
                var item = Items.Where(a => a.CategoryId == categoryId && a.Month == Month).FirstOrDefault();
                item.Count += count;
            }
            catch // chưa có trong giỏ -> truy vấn CSDL và bỏ vào giỏ
            {
                var db = new AppDbContext();
                var item = db.Price.Find(categoryId, Month);

                var flashSale = db.FlashSaleCategories.AsQueryable().Where(a => a.CategoryId == categoryId && a.Month == Month && a.FlashSales.TimeEnd >= DateTime.Now && a.FlashSales.TimeStart <= DateTime.Now).ToList();

                if (flashSale.Count > 0)
                {
                    Items.Add(new CartViewModel()
                    {
                        Name = item.Categories.Name,
                        CategoryId = item.CategoryId,
                        ImageId = item.Categories.ImageId,
                        Month = item.Month,
                        TypeOfAccount = type,
                        Count = count,
                        UserAccount = user,
                        UserPassword = pass
                    });
                }
                else
                {
                    Items.Add(new CartViewModel()
                    {
                        Name = item.Categories.Name,
                        CategoryId = item.CategoryId,
                        ImageId = item.Categories.ImageId,
                        Month = item.Month,
                        TypeOfAccount = type,
                        Count = count,
                        UserAccount = user,
                        UserPassword = pass
                    });
                }
            }
        }

        public void Remove(string categoryId, double Month)
        {
            var item = Items.Where(a => a.CategoryId == categoryId && a.Month == Month).FirstOrDefault();
            Items.Remove(item);
        }

        public void Update(string categoryId, double Month, bool IsPlus)
        {
            var item = Items.Single(i => i.CategoryId == categoryId && i.Month == Month);
            if (IsPlus)
            {
                item.Count++;
            }
            else
            {
                if (item.Count == 1)
                    Remove(categoryId, Month);
                else
                    item.Count--;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public double TotalPrice
        {
            get
            {
                return Items.Sum(p => p.Price * p.Count);
            }
        }
        public string TotalPriceShow
        {
            get
            {
                return string.Format("{0:0,0 đ}", TotalPrice);
            }
        }
    }
}