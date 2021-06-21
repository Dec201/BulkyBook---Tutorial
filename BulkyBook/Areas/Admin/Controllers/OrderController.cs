using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {

            OrderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperies: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderId == id, includeProperies: "Product")
            };

            return View(OrderDetailsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {


            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetailsVM.OrderHeader.Id,
                includeProperies: "ApplicationUser");


            if (stripeToken != null)
            {
                // process the payment
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Currency = "gbp",
                    Description = "Order ID : " + orderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                {
                    orderHeader.PaymentStatus = StaticDetails.PaymentStatusRejected;
                }
                else
                {
                    orderHeader.TransactionId = charge.BalanceTransactionId;
                }

                if (charge.Status.ToLower() == "succeeded")
                {
                    orderHeader.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    orderHeader.PaymentDate = DateTime.UtcNow;
                }


                _unitOfWork.Save();

            }

            // return RedirectToAction("Details", "Order", new {id = orderHeader.Id);

            return RedirectToAction(nameof(Details), "Order", new { id = orderHeader.Id });

        }











        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);

            orderHeader.OrderStatus = StaticDetails.StatusInProgress;

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }


        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder()
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == OrderDetailsVM.OrderHeader.Id);

            //binded property so can use OrderDetailsVM
            orderHeader.TrackingNumber = OrderDetailsVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailsVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            orderHeader.ShippingDate = DateTime.UtcNow;

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }


        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);

            if (orderHeader.PaymentStatus == StaticDetails.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                orderHeader.OrderStatus = StaticDetails.StatusRefunded;
                orderHeader.PaymentStatus = StaticDetails.StatusRefunded;

            }
            else
            {

                orderHeader.OrderStatus = StaticDetails.StatusCancelled;
                orderHeader.PaymentStatus = StaticDetails.StatusCancelled;

            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UpdateOrderDetails()
        {

            var orderHeaderDb = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == OrderDetailsVM.OrderHeader.Id);


            if (orderHeaderDb != null)
            {
                orderHeaderDb.Name = OrderDetailsVM.OrderHeader.Name;
                orderHeaderDb.PhoneNumber = OrderDetailsVM.OrderHeader.PhoneNumber;
                orderHeaderDb.StreetAddress = OrderDetailsVM.OrderHeader.StreetAddress;
                orderHeaderDb.City = OrderDetailsVM.OrderHeader.City;
                orderHeaderDb.State = OrderDetailsVM.OrderHeader.State;
                orderHeaderDb.PostalCode = OrderDetailsVM.OrderHeader.PostalCode;
                orderHeaderDb.OrderDate = OrderDetailsVM.OrderHeader.OrderDate;
                
                if(OrderDetailsVM.OrderHeader.Carrier != null)
                {
                    orderHeaderDb.Carrier = OrderDetailsVM.OrderHeader.Carrier;
                }

                if(OrderDetailsVM.OrderHeader.TrackingNumber != null)
                {
                    orderHeaderDb.TrackingNumber = OrderDetailsVM.OrderHeader.TrackingNumber;
                }

            }
            else
            {
                TempData["Error"] = "Order Details Failed Update";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", "Order", new { id = orderHeaderDb.Id });
            // return RedirectToAction(nameof(Details), orderHeaderDb);
            //return View(orderHeaderDb);


        }





        #region API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;


            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperies: "ApplicationUser");
            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claim.Value, includeProperies: "ApplicationUser");
            }


            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusInProgress ||
                                                                                            o.OrderStatus == StaticDetails.StatusApproved ||
                                                                                            o.OrderStatus == StaticDetails.StatusPending);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusCancelled ||
                                                                                            o.OrderStatus == StaticDetails.StatusRefunded ||
                                                                                            o.PaymentStatus == StaticDetails.PaymentStatusRejected);
                    break;
                default:
                    break;
            }



            return Json(new { data = orderHeaderList });
        }
        #endregion











    }
}
