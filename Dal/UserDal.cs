using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using easysql;
using Model;
namespace Dal
{
    public class UserDal:BaseDal<UserModel>
    {
        public UserDal() : base("user")
        {

        }

     
    }
}
