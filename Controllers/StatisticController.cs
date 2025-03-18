using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project3.Models;

namespace Project3.Controllers
{
    public class StatisticController : Controller
    {
        // GET: Statistic
        dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString8"].ConnectionString);
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StatisticBook()
        {
            List<SumSaleQuantity> lst = (from a in db.books
                                         join b in db.orderdetails on a.book_id equals b.book_id
                                         group b by b.book_id into g
                                         select new SumSaleQuantity
                                         {
                                             bookName = g.First().book.book_title,
                                             sumQuantity = g.Sum(x => x.quantity)
                                         }).ToList();
            return View(lst);
        }
    }
}