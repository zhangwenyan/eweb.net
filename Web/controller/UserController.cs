using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using Web.handler;
using Dal;
namespace Web.controller
{
    public class UserController:BaseController<UserModel>
    {
        private UserDal dal = new UserDal();
        public object hello(PageInfo<UserModel> pi,String name,HttpRequest req,UserModel um,int testId)
        {
            return dal.queryPage(pi);
        }

    }
}