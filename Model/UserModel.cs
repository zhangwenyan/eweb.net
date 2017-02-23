using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class UserModel:BaseModel
    {
        public int id { get; set; }
        public String username { get; set; }
        public String password { get; set; }
    }
}
