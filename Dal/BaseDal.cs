using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using easysql;
using Model;
namespace Dal
{
    public abstract class BaseDal<T> where T : new()
    {
        protected BaseDBHelper dh = DBHelperFactory.Create(ConfigurationManager.AppSettings["dbType"] ?? "mysql", ConfigurationManager.AppSettings["connstr"]);
        protected String tbname;
        protected BaseDal(string tbname)
        {
            this.tbname = tbname;
        }
        public virtual List<T> queryPage(T bean, int page, int rows, out int total, params Restrain[] restrains)
        {
            return dh.QueryByPage<T>(tbname, bean, page, rows, out total, restrains);
        }

        public virtual PageResultInfo queryPage(PageInfo<T> pi, params Restrain[] restrains)
        {
            int total = 0;
            var list = queryPage(pi.query, pi.page, pi.rows, out total, restrains);
            return new PageResultInfo()
            {
                total = total,
                rows = list
            };
        }



        public virtual void add(T model)
        {
            dh.Add(tbname, model);
        }

        public virtual void modify(T model)
        {
            dh.Modify(tbname, model);
        }

        public virtual void del(string ids)
        {
            using (BaseDatabase db = dh.CreateDatabaseAndOpen())
            {
                var idArr = ids.Split(',');
                foreach (var id in idArr)
                {
                    db.DelById(tbname, int.Parse(id));
                }
            }
        }


    }
}
