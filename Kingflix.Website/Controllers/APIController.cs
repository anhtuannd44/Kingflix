using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Models;

namespace Kingflix.Controllers
{
    public class APIController : Controller
    {
        private readonly IOrderService _orderService;
        public APIController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public void PaymentAPISuccess()
        {
            string data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();

            JObject json = JObject.Parse(data);

            string CodeResult = (string)json["order"]["stat"];

            if (CodeResult == "c")
            {
                _orderService.UpdateOrder((string)json["order"]["mrc_order_id"], OrderStatus.Done, null, false);
            }
        }

    }
}