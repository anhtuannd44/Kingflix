using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Models;
using Kingflix.Utilities;

namespace Kingflix.Controllers
{
    public class APIController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ISettingService _settingService;
        public APIController(
            IOrderService orderService,
            IUserService userService,
            ISettingService settingService
            )
        {
            _orderService = orderService;
            _userService = userService;
            _settingService = settingService;
        }

        [HttpPost]
        public void PaymentAPISuccess()
        {
            string data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();

            JObject json = JObject.Parse(data);

            string resultStat = (string)json["order"]["stat"];
            double amount = (double)json["order"]["total_amount"];
            int bpm_id = (int)json["order"]["bpm_id"];
            string orderId = (string)json["order"]["mrc_order_id"];
            if (resultStat == "c")
            {
                var order = _orderService.GetOrderById(orderId);
                double refundAmount = 0; 
                if (bpm_id == 128)
                {
                    refundAmount = amount * 2.4 + 2000;
                }
                else
                {
                    foreach (var item in Const.ATM_BAOKIM_ID_LIST)
                    {
                        if (item == bpm_id)
                        {
                            refundAmount = amount * 1.1 + 1250;           
                            break;
                        }
                    }
                }
                if (refundAmount >0)
                {
                    var refundItem = _settingService.GetRefundItem();
                    if (refundItem.Value.HasValue)
                        if (refundAmount > refundItem.Value.Value)
                            refundAmount = refundItem.Value.Value;
                    _userService.Refund(order.UserId, refundAmount);
                }
                _orderService.UpdateOrder(orderId, OrderStatus.Done, null, false);
            }
        }

    }
}