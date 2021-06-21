using Braintree;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrainTreeController : Controller
    {

        public IBrainTreeGate _brainTree { get; set; }

        public BrainTreeController(IBrainTreeGate brainTree)
        {
            _brainTree = brainTree;
        }



        public IActionResult Index()
        {

            var gateway = _brainTree.GetGateway();
            var clientToken = gateway.ClientToken.Generate();

            // use ViewModel
            ViewBag.ClientToken = clientToken;

            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(IFormCollection collection)
        {
            Random rnd = new Random();

            string nonceFromTheClient = collection["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = rnd.Next(1, 100),
                PaymentMethodNonce = nonceFromTheClient,
                OrderId = "55501",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _brainTree.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            if(result.Target.ProcessorResponseText == "Approved")
            {
                TempData["Success"] = "Transaction was successful Transaction ID "
                    + result.Target.Id + ", Amount Charged : $" + result.Target.Amount;
            }

            return RedirectToAction("Index");

        }






    }
}
