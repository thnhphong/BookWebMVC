using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project3.Models;

namespace Project3.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Checkout(int bookId)
        {
            return RedirectToAction("Index", "Checkout");
        }

        dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString8"].ConnectionString);
        // GET: Order
        public ActionResult Orders()
        {
            // var items = db.orders.OrderByDescending(x => x.order_date).ToList();
            List<Order> lst = (from a in db.orders
                               join b in db.orderdetails on a.order_id equals b.order_id
                               group b by b.order_id into g
                               select new Order
                               {
                                   orderID = g.First().order.order_id,
                                   customerID = g.First().order.customer_id.ToString(),
                                   isShip = g.First().order.isship.Value,
                                   isPayment = g.First().order.ispayment.Value,
                                   deliveryDate = g.First().order.delivery_date.Value,
                                   orderDate = g.First().order.order_date.Value,
                                   total = g.Sum(x => x.price)
                               }).ToList();
            return View(lst);
        }
        public ActionResult Detail(int id)
        {
            var orderDetails = (from od in db.orderdetails
                                join b in db.books on od.book_id equals b.book_id
                                where od.order_id == id
                                select new OrderDetailViewModel
                                {
                                    OrderID = od.order_id, // Chuyển int? -> int
                                    BookName = b.book_title,
                                    Quantity = od.quantity.GetValueOrDefault(), // Chuyển int? -> int
                                    Price = od.price.GetValueOrDefault(), // Chuyển decimal? -> decimal
                                    Total = (od.quantity ?? 0) * (od.price ?? 0) // Xử lý null an toàn
                                }).ToList();
            return View(orderDetails);
        }
    }
}