using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.RedemptionViewModels;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<RedemptionViewModel> models = new List<RedemptionViewModel>()
            {
                new RedemptionViewModel{ Id =1,Selected=false,LineNo = 1,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =2,Selected=false,LineNo = 2,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =3,Selected=true,LineNo = 3,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =4,Selected=false,LineNo = 4,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =5,Selected=false,LineNo = 5,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =6,Selected=false,LineNo = 6,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =7,Selected=false,LineNo = 7,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
                new RedemptionViewModel{ Id =8,Selected=false,LineNo = 8,RedeemDate = DateTime.Now, DealerName = "Dealer 1", CouponPrice = 300, Email = "de@gmial.com", Quantity = 10 },
            };

            return View(models);
        }
    }
}
