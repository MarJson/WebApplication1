using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public class SigninUser
    {
        public Guid userId { get; set; }

        public string userName { get; set; }
    }

    public class UserHelper
    {
        private static string sessionName = "WxvetEnterpriseSiteAdmin";

        /// <summary>
        /// 设置系统用户的session
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRoles"></param>
        public static void SetSigninUser(Guid userId, string userName)
        {
            SigninUser luser = new SigninUser() { userId = userId, userName = userName };

            HttpContext.Current.Session[sessionName] = luser;
        }

        /// <summary>
        /// 清除系统用户session
        /// </summary>
        public static void ClearSigninUser()
        {
            HttpContext.Current.Session[sessionName] = null;
        }

        /// <summary>
        /// 获取登录的系统用户对象
        /// </summary>
        /// <returns></returns>
        public static SigninUser GetSigninUser
        {
            get
            {
                if (HttpContext.Current.Session[sessionName] == null)
                    return null;

                return HttpContext.Current.Session[sessionName] as SigninUser;
            }
        }

    }
}