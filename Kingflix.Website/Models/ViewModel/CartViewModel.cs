using Kingflix.Domain.Enumerables;
using Kingflix.Services.Data;
using System;
using System.Linq;

namespace Kingflix.Models.ViewModel
{
    public class PriceDetailsViewModel
    {
        public double Price { get; set; }
        public string PriceShow { get; set; }
        public string TotalShow { get; set; }

    }
    public class CartViewModel
    {
        public string CategoryId { get; set; }

        public TypeOfAccount TypeOfAccount { get; set; }
        public string Name { get; set; }

        public string ImageId { get; set; }

        public double Month { get; set; }
        public int Count { get; set; }

        public string UserAccount { get; set; }
        public string UserPassword { get; set; }

        public double Price
        {
            get
            {
                var db = new AppDbContext();
                var flashSale = db.FlashSaleCategories.AsQueryable().Where(a => a.CategoryId == CategoryId && a.Month == Month && a.FlashSales.TimeEnd >= DateTime.Now && a.FlashSales.TimeStart <= DateTime.Now).ToList();
                if (flashSale.Count > 0)
                    return flashSale.OrderBy(a => a.PriceSale).FirstOrDefault().PriceSale;
                return db.Price.Find(CategoryId, Month).SetPrice;
            }
            set
            {

            }
        }
        public string PriceShow
        {
            get
            {
                return string.Format("{0:0,0 đ}", Price);
            }
        }
        public string TotalPrice
        {
            get
            {
                return string.Format("{0:0,0 đ}", Price * Count);
            }

        }
    }
}