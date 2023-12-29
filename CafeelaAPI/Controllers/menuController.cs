using BAL.Repositories;
using DAL.DBEntities;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ZSixRestaurantAPI.Controllers
{
    [RoutePrefix("api")]
    public class menuController : ApiController
    {
        menuRepository repo;
        public menuController()
        {
            repo = new menuRepository(new db_a82b87_zsixrestaurantEntities());
        }

        /// <summary>
        /// List of categories and for each category , item list is inherited
        /// </summary>
        /// <param name="brandID"></param>
        /// <returns></returns>
        /// 
        [Route("menu/{brandID}")]
        public RspMenu GetMenu(string brandID)
        {
            return repo.GetMenuV2(int.Parse(brandID));
        }
    }
}
