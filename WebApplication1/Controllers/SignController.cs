using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
using Zhongyu.Data;
using Zhongyu.Data.Extensions;

namespace WebApplication1.Controllers
{
    public class SignController : Controller
    {
        // GET: Sign
  
        public ActionResult In()
        {
            
            ViewBag.url = "/Home/Index";
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult In(string uname, string pword)
        {
            OperationResult r = new OperationResult();
            if (uname.IsNullOrEmpty() || pword.IsNullOrEmpty())
            {
                r.Message = "用户名和密码不能为空";
                r.ResultType = OperationResultType.ValidError;
            }
            else
            {
                string MD5Pword = Zhongyu.Common.EncryptHelper.MD5Encrypt(pword, 32);
                UserDto user = DapperService.SqlHelp.UserLogin(uname, pword);


              //  AdminUser item = iauser.Entities.Where(p => p.Name.Equals(uname) && p.Password.Equals(MD5Pword)).FirstOrDefault();



                if (user==null)
                {
                    r.Message = "用户名或密码错误";
                    r.ResultType = OperationResultType.Error;
                }
                else
                {


                    r.ResultType = OperationResultType.Success;
                  
                    UserHelper.SetSigninUser(user.UserId,user.user_name);
                }

            }
            return Json(r);
        }



        public ActionResult Out()
        {
            UserHelper.ClearSigninUser();
            return RedirectToAction("In", "Sign", new { Area = "" });
        }



    }
}