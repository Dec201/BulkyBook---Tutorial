using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBook.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public class UserController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly ApplicationDbContext _applicationDbContext;


        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext)
        {
            _unitOfWork = unitOfWork;
            _applicationDbContext = applicationDbContext;
        }



        public IActionResult Index()
        {
            return View();
        }



        // region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _applicationDbContext.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _applicationDbContext.UserRoles.ToList();
            var roles = _applicationDbContext.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }



        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _applicationDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }


            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked, we will unlock the account
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _applicationDbContext.SaveChanges();
            return Json(new { success = true, message = "Operation successful" });

        }




    }
}
