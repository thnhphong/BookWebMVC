using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project3.Models;

namespace Project3.Controllers
{
    public class CartController : Controller
    {
        dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString8"].ConnectionString);

        public List<Cart> getCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }

        public ActionResult AddCart(int id, string strURL)
        {
            List<Cart> lstCart = getCart();
            Cart product = lstCart.Find(n => n.book_id == id);
            if (product == null)
            {
                product = new Cart(id);
                lstCart.Add(product);
                return Redirect(strURL);
            }
            else
            {
                product.quantity++;
                return Redirect(strURL);
            }
        }

        private int sumQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tsl = lstCart.Sum(n => n.quantity);
            }
            return tsl;
        }

        private int sumProductQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tsl = lstCart.Count;
            }
            return tsl;
        }

        private decimal Total()
        {
            decimal tt = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tt = lstCart.Sum(n => n.Total);
            }
            return tt;
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return View(lstCart);
        }

        public ActionResult CartPartial()
        {
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return PartialView();
        }

        public ActionResult CartDelete(int id)
        {
            List<Cart> lstCart = getCart();
            Cart product = lstCart.SingleOrDefault(n => n.book_id == id);
            if (product != null)
            {
                lstCart.RemoveAll(n => n.book_id == id);
                return Redirect("Cart");
            }
            return RedirectToAction("Cart");
        }
        public ActionResult CartUpdate(int id, FormCollection collection)
        {
            List<Cart> lstCart = getCart();
            Cart product = lstCart.SingleOrDefault(n => n.book_id == id);
            if (product != null)
            {
                product.quantity = int.Parse(collection["txtQuantity"].ToString());
            }
            return RedirectToAction("Cart");
        }

        public ActionResult AllCartDelete()
        {
            List<Cart> lstCart = getCart();
            lstCart.Clear();
            return RedirectToAction("Cart");
        }

        //place an order 
        [HttpGet]
        public ActionResult PlaceOrder()
        {
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("Login", "User");
            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Book");
            }
            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();

            return View(lstCart);
        }

        public ActionResult PlaceOrder(FormCollection collection)
        {
            order dh = new order();
            customer kh = (customer)Session["User"];
            if (Session["User"] == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Session[\"User\"] is null. No user logged in.");
            }
            else
            {
                var user = (customer)Session["User"];
                System.Diagnostics.Debug.WriteLine("✅ Logged-in user: " + user.customer_name + " | Email: " + user.email);
            }

            book s = new book();
            List<Cart> gh = getCart();

            var delivery_date = String.Format("{0:MM/dd/yyyy}", collection["delivery_date"]);

            dh.customer_id = kh.customer_id;
            dh.order_date = DateTime.Now;
            dh.delivery_date = DateTime.Parse(delivery_date);
            dh.isship = false;
            dh.ispayment = false;

            db.orders.InsertOnSubmit(dh);
            db.SubmitChanges();

            var strSanPham = "";

            var thanhTien = decimal.Zero;
            var tongTien = decimal.Zero;

            foreach (var item in gh)
            {
                orderdetail ctdh = new orderdetail();
                ctdh.order_id = dh.order_id;
                ctdh.book_id = item.book_id;
                ctdh.quantity = item.quantity;
                ctdh.price = (decimal)item.price;

                // Cập nhật số lượng sách trong kho
                s = db.books.Single(n => n.book_id == item.book_id);
                s.quantity_in_stock -= (int)ctdh.quantity;

                // Thêm vào bảng orderdetails
                db.orderdetails.InsertOnSubmit(ctdh);

                // Create email content
                // Create table row
                strSanPham += $"<tr><td>{item.book_name}</td><td>{item.quantity}</td><td>{item.price:N2}</td><td>{item.Total:N2}</td></tr>";
                thanhTien += (decimal)item.price * item.quantity;
            }

            db.SubmitChanges();
            tongTien = thanhTien;

            // Đọc nội dung file email mẫu (send2.html) từ thư mục Content/templates
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));

            // Thay thế các placeholder trong template bằng dữ liệu thực
            contentCustomer = contentCustomer.Replace("{{MaDon}}", dh.order_id.ToString());
            contentCustomer = contentCustomer.Replace("{{NgayDatHang}}", dh.delivery_date.Value.ToString("MM/dd/yyyy"));
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", kh.customer_name);
            contentCustomer = contentCustomer.Replace("{{Phone}}", kh.numberphone);
            contentCustomer = contentCustomer.Replace("{{Email}}", kh.email);
            contentCustomer = contentCustomer.Replace("{{DiaChi}}", kh.address);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhTien.ToString("N2"));
            contentCustomer = contentCustomer.Replace("{{TongTien}}", tongTien.ToString("N2"));

            // Gửi email xác nhận cho khách hàng
            Common.Common.SendMail("ShopOnline", "Đơn hàng #" + dh.order_id.ToString(), contentCustomer, kh.email);
            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            return RedirectToAction("ConfirmOrder", "Cart");
        }
        public ActionResult ConfirmOrder()
        {
            return View();
        }

        
    }
}