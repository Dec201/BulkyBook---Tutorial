﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Upsert(int? id)
        {

            CoverType coverType = new CoverType();

            if (id == null)
            {
                return View(coverType);
            }


            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            coverType = _unitOfWork.SP_Call.Single<CoverType>(StaticDetails.Proc_CoverType_Get, parameter);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);

        }




        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {

            if(ModelState.IsValid)
            {

                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);


                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(StaticDetails.Proc_CoverType_Create, parameter);
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(StaticDetails.Proc_CoverType_Update, parameter);
                }


                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));


            }

            return View(coverType);

        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var dataFromDb = _unitOfWork.SP_Call.List<CoverType>(StaticDetails.Proc_CoverType_GetAll, null);
            return Json(new { data = dataFromDb });
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(StaticDetails.Proc_CoverType_Get, parameter);

            if(objFromDb == null)
            {
                return Json(new { success = false, message = "There was an issue deleting the Cover Type" });
            }

            _unitOfWork.SP_Call.Execute(StaticDetails.Proc_CoverType_Delete, parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Cover Type deleted !" });

        }





    }
}
