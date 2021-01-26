using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Models.ViewModel;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public UserProductController(
            IProductService productService,
            IUserService userService,
            IOrderService orderService
            )
        {
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetUserActionChart(int month)
        {
            var result = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                {
                    new Datasets()
                    {
                        label = "Mua mới",
                    },
                    new Datasets()
                    {
                        label = "Đặt hàng nhưng hủy (hoặc chưa thanh toán)",
                    },
                    new Datasets()
                    {
                        label = "Vào trang nhưng chưa đặt hàng",
                    }
                }
            };

            var order = _orderService.GetOrderList();
            var orderButNotSuccess = new List<Order>();
            var user = _userService.GetUserList();
            var notOrderUser = new List<AppUser>();
            var notOrder = notOrderUser.Select(a => a.TimeStep2).ToList();


            foreach (var item in user)
            {
                var check = order.Where(a => a.UserId == item.Id).Count();
                if (check == 0 && item.TimeStep2.Date <= DateTime.Today && item.TimeStep2 >= DateTime.Today.AddMonths(-1).Date)
                    notOrderUser.Add(item);
            }

            if (month == 1)
            {
                int thisYear = DateTime.Today.Year;
                int thisMonth = DateTime.Today.Month;
                var newOrder = order.Where(a => a.Status == OrderStatus.Done)
                                       .GroupBy(a => a.UserId)
                                       .Select(a => new { count = a.Count(), dateOrder = a.FirstOrDefault().DateCreated })
                                       .Where(a => a.count == 1 && a.dateOrder.Year == thisYear && a.dateOrder.Month == thisMonth)
                                       .ToList();

                foreach (var item in order.Where(a => a.Status != OrderStatus.Done).ToList())
                {
                    var check = order.Where(a => a.UserId == item.UserId && a.Status == OrderStatus.Done).Count();
                    if (check == 0)
                        orderButNotSuccess.Add(item);
                }
                var orderCancel = orderButNotSuccess.GroupBy(a => a.UserId)
                                                        .Select(a => new { dateOrder = a.FirstOrDefault().DateCreated })
                                                        .Where(a => a.dateOrder.Year == thisYear && a.dateOrder.Month == thisMonth)
                                                        .ToList();



                int days = DateTime.DaysInMonth(thisYear, thisMonth);
                result.labels = new string[days];
                result.datasets[0].data = new double[days];
                result.datasets[1].data = new double[days];
                result.datasets[2].data = new double[days];
                result.datasets[0].backgroundColor = new string[days];
                result.datasets[1].backgroundColor = new string[days];
                result.datasets[2].backgroundColor = new string[days];
                result.datasets[0].borderColor = new string[1] { "#5aaf28" };
                result.datasets[1].borderColor = new string[1] { "#e50914" };
                result.datasets[2].borderColor = new string[1] { "#edcb1f" };
                for (int i = 0; i < days; i++)
                {
                    try
                    {
                        result.labels[i] = (i + 1).ToString() + "/" + thisMonth + "/" + thisYear;
                        result.datasets[0].data[i] = newOrder.Count != 0 ? newOrder.Where(a => a.dateOrder.Day == i).Count() : 0;
                        result.datasets[1].data[i] = orderCancel.Count != 0 ? orderCancel.Where(a => a.dateOrder.Day == i).Count() : 0;
                        result.datasets[2].data[i] = notOrder.Count != 0 ? notOrder.Where(a => a.Day == i).Count() : 0;
                        result.datasets[0].backgroundColor[i] = "#5aaf28";
                        result.datasets[1].backgroundColor[i] = "#e50914";
                        result.datasets[2].backgroundColor[i] = "#edcb1f";
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                var today = DateTime.Today;
                var firstMonth = today.AddMonths(-month);
                var firstDay = new DateTime(firstMonth.Year, firstMonth.Month, 1);
                result.labels = new string[month];
                result.datasets[0].data = new double[month];
                result.datasets[1].data = new double[month];
                result.datasets[2].data = new double[month];
                result.datasets[0].backgroundColor = new string[month];
                result.datasets[1].backgroundColor = new string[month];
                result.datasets[2].backgroundColor = new string[month];
                result.datasets[0].borderColor = new string[1] { "#5aaf28" };
                result.datasets[1].borderColor = new string[1] { "#e50914" };
                result.datasets[2].borderColor = new string[1] { "#edcb1f" };

                var newOrder = order.Where(a => a.Status == OrderStatus.Done)
                                       .GroupBy(a => a.UserId)
                                       .Select(a => new { count = a.Count(), dateOrder = a.FirstOrDefault().DateCreated })
                                       .Where(a => a.count == 1 && a.dateOrder.Date <= today.Date && a.dateOrder.Date >= firstDay.Date)
                                       .ToList();
                foreach (var item in order.Where(a => a.Status != OrderStatus.Done).ToList())
                {
                    var check = order.Where(a => a.UserId == item.UserId && a.Status == OrderStatus.Done).Count();
                    if (check == 0)
                        orderButNotSuccess.Add(item);
                }
                var orderCancel = orderButNotSuccess.GroupBy(a => a.UserId)
                                                        .Select(a => new { dateOrder = a.FirstOrDefault().DateCreated })
                                                        .Where(a => a.dateOrder.Date <= today.Date && a.dateOrder.Date >= firstDay.Date)
                                                        .ToList();

                for (int i = 0; i < month; i++)
                {
                    var monthItem = today.AddMonths(-month + i + 1);
                    result.labels[i] = monthItem.Month.ToString() + "/" + monthItem.Year.ToString();
                    result.datasets[0].data[i] = newOrder.Where(a => a.dateOrder.Month == monthItem.Month).Count();
                    result.datasets[0].backgroundColor[i] = "#5aaf28";


                    result.datasets[1].data[i] = orderCancel.Where(a => a.dateOrder.Month == monthItem.Month).Count();
                    result.datasets[1].backgroundColor[i] = "#e50914";

                    result.datasets[2].data[i] = notOrder.Where(a => a.Month == monthItem.Month).Count();
                    result.datasets[2].backgroundColor[i] = "#edcb1f";

                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult GetUserInformationChart()
        {
            var result = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                {
                    new Datasets()
                    {
                        data = new double[7],
                        label = "Nam",
                        borderColor = new string[1] { "rgba(90, 175, 40, .1)" },
                        backgroundColor = new string[7] {"rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)","rgba(90, 175, 40, .1)"}
                    },
                    new Datasets()
                    {
                        data = new double[7],
                        label = "Nữ",
                        borderColor = new string[1] { "rgba(229, 9, 20,.1)" },
                        backgroundColor = new string[7] { "rgba(229, 9, 20,.1)" , "rgba(229, 9, 20,.1)", "rgba(229, 9, 20,.1)", "rgba(229, 9, 20,.1)", "rgba(229, 9, 20,.1)", "rgba(229, 9, 20,.1)", "rgba(229, 9, 20,.1)"},
                    },
                },
                labels = new string[7] { "13-17", "18-24", "25-34", "34-44", "45-54", "55-64", "65+" }
            };

            var user = _userService.GetUserList();
            var thisYear = DateTime.Today.Year;
            var man = user.Where(a => a.Gentle == Gentle.Man && a.Birthday != null).Select(a => a.Birthday ?? DateTime.MinValue).ToList();
            var woman = user.Where(a => a.Gentle == Gentle.Women && a.Birthday != null).Select(a => a.Birthday ?? DateTime.MinValue).ToList();
            result.datasets[0].data[0] = man.Where(a => thisYear - a.Year >= 13 && thisYear - a.Year <= 17).Count();
            result.datasets[0].data[1] = man.Where(a => thisYear - a.Year >= 18 && thisYear - a.Year <= 24).Count();
            result.datasets[0].data[2] = man.Where(a => thisYear - a.Year >= 24 && thisYear - a.Year <= 34).Count();
            result.datasets[0].data[3] = man.Where(a => thisYear - a.Year >= 35 && thisYear - a.Year <= 44).Count();
            result.datasets[0].data[4] = man.Where(a => thisYear - a.Year >= 45 && thisYear - a.Year <= 54).Count();
            result.datasets[0].data[5] = man.Where(a => thisYear - a.Year >= 55 && thisYear - a.Year <= 64).Count();
            result.datasets[0].data[6] = man.Where(a => thisYear - a.Year >= 65).Count();

            result.datasets[1].data[0] = woman.Where(a => thisYear - a.Year >= 13 && thisYear - a.Year <= 17).Count();
            result.datasets[1].data[1] = woman.Where(a => thisYear - a.Year >= 18 && thisYear - a.Year <= 24).Count();
            result.datasets[1].data[2] = woman.Where(a => thisYear - a.Year >= 24 && thisYear - a.Year <= 34).Count();
            result.datasets[1].data[3] = woman.Where(a => thisYear - a.Year >= 35 && thisYear - a.Year <= 44).Count();
            result.datasets[1].data[4] = woman.Where(a => thisYear - a.Year >= 45 && thisYear - a.Year <= 54).Count();
            result.datasets[1].data[5] = woman.Where(a => thisYear - a.Year >= 55 && thisYear - a.Year <= 64).Count();
            result.datasets[1].data[6] = woman.Where(a => thisYear - a.Year >= 65).Count();

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult GetRevenue(string categoryId, int? month, int? timeView)
        {
            var result = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                {
                    new Datasets()
                    {
                        label = "Doanh thu",
                        yAxisID = "Revenue"
                    },
                    new Datasets()
                    {
                        label = "Doanh số",
                        yAxisID = "Sales"
                    }
                }
            };
            var order = _orderService.GetOrderDetailsList(a => a.Orders.Status == OrderStatus.Done).ToList();
            if (!string.IsNullOrEmpty(categoryId))
                order = order.Where(a => a.CategoryId == categoryId).ToList();
            if (month != null)
                order = order.Where(a => a.Month == (month ?? 0)).ToList();

            if (timeView == 0)
            {
                result.labels = new string[7];
                var today = DateTime.Today;
                result.datasets[0].data = new double[7];
                result.datasets[0].backgroundColor = new string[7];
                result.datasets[0].borderColor = new string[7];

                result.datasets[1].data = new double[7];
                result.datasets[1].backgroundColor = new string[7];
                result.datasets[1].borderColor = new string[7];
                for (int i = 0; i < 7; i++)
                {
                    var date = today.AddDays(i - 6);
                    result.labels[i] = date.ToString("dd/MM/yyyy");
                    result.datasets[0].data[i] = order.Where(a => a.Orders.DateConfirmShow.Date == date).Sum(a=>a.Orders.Price);
                    result.datasets[0].backgroundColor[i] = "rgba(90, 175, 40, .1)";
                    result.datasets[0].borderColor[i] = "rgba(90, 175, 40, .4)";

                    result.datasets[1].data[i] = order.Where(a => a.Orders.DateConfirmShow.Date == date).Sum(a=>a.Count);
                    result.datasets[1].backgroundColor[i] = "rgba(25, 138, 227,.1)";
                    result.datasets[1].borderColor[i] = "rgba(25, 138, 227,.4)";
                }
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetSales(string type)
        {
            var order = _orderService.GetOrderDetailsList(a => a.Orders.Status == OrderStatus.Done);
            var result1 = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                        {
                            new Datasets()
                            {
                                data = new double[2],
                                borderColor = new string[2] { "#fff", "#fff"},
                                backgroundColor = new string[2] { "rgba(229, 9, 20,.9)", "rgba(90, 175, 40, .9)" }
                            },
                        }
            };
            var result2 = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                        {
                            new Datasets()
                            {
                                data = new double[4],
                                borderColor = new string[4] { "#fff", "#fff",  "#fff", "#fff"},
                                backgroundColor = new string[4] { "rgba(229, 9, 20,.9)", "rgba(90, 175, 40, .9)", "rgba(237, 203, 31, .9)", "rgba(25, 138, 227, .9)" }
                            },
                        },
                labels = new string[4] { "1 tháng", "3 tháng", "6 tháng", "12 tháng" }
            };

            switch (type)
            {
                case "salesCategory":
                    var result = new ChartViewModel()
                    {
                        datasets = new List<Datasets>()
                        {
                            new Datasets()
                            {
                                data = new double[3],
                                borderColor = new string[3] { "#fff", "#fff", "#fff" },
                                backgroundColor = new string[3] { "rgba(229, 9, 20, .9)", "rgba(90, 175, 40, .9)", "rgba(237, 203, 31, .9)" }
                            },
                        },
                        labels = new string[3] { "Ngắn hạn", "Dài hạn", "Gia đình" }
                    };
                    var netflix1 = order.Where(a => a.CategoryId == Const.NETFLIX1).Sum(a => a.Count);
                    var netflix2 = order.Where(a => a.CategoryId == Const.NETFLIX2).Sum(a => a.Count);
                    var netflix3 = order.Where(a => a.CategoryId == Const.NETFLIX3).Sum(a => a.Count);
                    result.datasets[0].data[0] = netflix1;
                    result.datasets[0].data[1] = netflix2;
                    result.datasets[0].data[2] = netflix3;
                    return Json(result, JsonRequestBehavior.DenyGet);

                case "salesNetflix1":
                    result1.labels = new string[2] { "1 tháng", "3 tháng" };
                    var category1 = order.Where(a => a.CategoryId == Const.NETFLIX1);
                    result1.datasets[0].data[0] = category1.Where(a => a.Month == 1).Sum(a => a.Count);
                    result1.datasets[0].data[1] = category1.Where(a => a.Month == 3).Sum(a => a.Count);
                    return Json(result1, JsonRequestBehavior.DenyGet);

                case "salesNetflix2":

                    var category2 = order.Where(a => a.CategoryId == Const.NETFLIX2);
                    result2.datasets[0].data[0] = category2.Where(a => a.Month == 1).Sum(a => a.Count);
                    result2.datasets[0].data[1] = category2.Where(a => a.Month == 3).Sum(a => a.Count);
                    result2.datasets[0].data[2] = category2.Where(a => a.Month == 6).Sum(a => a.Count);
                    result2.datasets[0].data[3] = category2.Where(a => a.Month == 12).Sum(a => a.Count);
                    return Json(result2, JsonRequestBehavior.DenyGet);

                case "salesNetflix3":
                    var category3 = order.Where(a => a.CategoryId == Const.NETFLIX3);
                    result2.datasets[0].data[0] = category3.Where(a => a.Month == 1).Sum(a => a.Count);
                    result2.datasets[0].data[1] = category3.Where(a => a.Month == 3).Sum(a => a.Count);
                    result2.datasets[0].data[2] = category3.Where(a => a.Month == 6).Sum(a => a.Count);
                    result2.datasets[0].data[3] = category3.Where(a => a.Month == 12).Sum(a => a.Count);
                    return Json(result2, JsonRequestBehavior.DenyGet);
                case "salesUser":
                    var today = DateTime.Today;
                    var userProfile = _productService.GetProfileList();
                    result1.datasets[0].data[0] = userProfile.Where(a => a.UserId != null && a.DateEnd >= today).Count();
                    result1.datasets[0].data[1] = order.Count() - result1.datasets[0].data[0];
                    result1.labels = new string[2] { "Hoạt động bình thường", "Hết hạn" };
                    return Json(result1, JsonRequestBehavior.DenyGet);

            }
            return Json(result2, JsonRequestBehavior.DenyGet);
        }
    }
}
