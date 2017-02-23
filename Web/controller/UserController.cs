using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using Web.handler;
namespace Web.controller
{
    public class UserController:BaseController<UserModel>
    {
        public object hello(PageInfo<UserModel> pi,String name,HttpRequest req,UserModel um,int testId)
        {
            return new
            {
                msg = um.id+"," + um.username + "," + um.password
            };
        }

    }
}