using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using Project3.Models;

namespace Project3.Controllers
{
    public class BookController : Controller
    {
        dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString8"].ConnectionString);
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.Keyword = searchString;
            if (page == null) page = 1;
            var all_book = (from books in db.books select books).OrderBy(x => x.book_id);
            if (!string.IsNullOrEmpty(searchString))
                all_book = (IOrderedQueryable<book>)all_book.Where(s => s.book_title.Contains(searchString));
            
            int pageSize = 3;
            int pageNum = page ?? 1;
            return View(all_book.ToPagedList(pageNum, pageSize));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
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

        public ActionResult Home(int? size, int? page)
        {
            //dropdown list
            ViewBag.sumQuantity = sumQuantity();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });
            items.Add(new SelectListItem { Text = "24", Value = "24" });
            items.Add(new SelectListItem { Text = "48", Value = "48" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
                    break;
            }
            ViewBag.size = items;
            ViewBag.currentSize = size;
            if (page == null) page = 1;
            var all_book = from book in db.books select book;
            int pageSize = (size ?? 3);
            int pageNum = page ?? 1;

            return View(all_book.ToPagedList(pageNum, pageSize));
        }


        //public ActionResult Detail(int id)
        //{
        //    var book = db.books.FirstOrDefault(b => b.book_id == id);
        //    if (book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(book);
        //}

                //public ActionResult Edit(int id)
                //{
                //    var book = db.books.SingleOrDefault(b => b.book_id == id);
                //        if (book == null)
                //    {
                //        return HttpNotFound();
                //    }

                //    var publisherName = db.publishers
                //        .Where(p => p.publisher_id == book.publisher_id)
                //        .Select(p => p.publisher_name)
                //        .FirstOrDefault();

                //    ViewBag.PublisherName = publisherName ?? "N/A";

                //    return View(book);
                //}

                //[HttpPost]
                //[ValidateAntiForgeryToken]
                //public ActionResult Edit(int id, book updatedBook, HttpPostedFileBase imageFile)
                //{
                //    if (ModelState.IsValid)
                //    {
                //        var book = db.books.FirstOrDefault(b => b.book_id == id);
                //        if (book == null)
                //        {
                //            return HttpNotFound();
                //        }

                //        book.book_title = updatedBook.book_title;
                //        book.price = updatedBook.price;
                //        book.description = updatedBook.description;

                //        if (imageFile != null && imageFile.ContentLength > 0)
                //        {
                //            string fileName = Path.GetFileName(imageFile.FileName);
                //            string path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                //            imageFile.SaveAs(path);
                //            book.image = fileName;
                //        }

                //        db.SubmitChanges();
                //        return RedirectToAction("Index");
                //    }
                //    return View(updatedBook);
                //}

                //[HttpPost]
                //[ValidateAntiForgeryToken]

                //public ActionResult Delete(int id)
                //{
                //    using (dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString1"].ConnectionString))
                //    {
                //        db.ExecuteCommand("DELETE FROM Books WHERE book_id = {0}", id);
                //    }
                //    return RedirectToAction("Index");
                //}

                //public ActionResult Create()
                //{
                //    return View();
                //}

                //[HttpPost]
                //[ValidateAntiForgeryToken]
                //public ActionResult Create(book newBook)
                //{
                //    using (dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString3"].ConnectionString))
                //    {
                //        if (ModelState.IsValid)
                //        {
                //            if (newBook.update_date == DateTime.MinValue)
                //            {
                //                newBook.update_date = DateTime.Now;
                //            }
                //            db.books.InsertOnSubmit(newBook);
                //            db.SubmitChanges();
                //            return RedirectToAction("Index");
                //        }
                //    }
                //    return View(newBook);
                //}
        public ActionResult Detail(int id, string returnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Home") : returnUrl;
            var D_book = db.books.Where(m => m.book_id == id).First();
            return View(D_book);
        }

        public ActionResult Edit(int id)
        {
            var E_book = db.books.Where(m => m.book_id == id).First();
            return View(E_book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(book E_book, HttpPostedFileBase fileUpload, string imagePath)
        {
            var existingBook = db.books.SingleOrDefault(b => b.book_id == E_book.book_id);
            if (existingBook == null)
            {
                return HttpNotFound();
            }
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(fileUpload.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                fileUpload.SaveAs(path);
                existingBook.image = fileName;
            }
            else
            {
                existingBook.image = imagePath;
            }

            existingBook.publisher_id = E_book.publisher_id;
            existingBook.book_title = E_book.book_title;
            existingBook.price = E_book.price;
            existingBook.quantity_in_stock = E_book.quantity_in_stock;
            existingBook.update_date = E_book.update_date;
            existingBook.description = E_book.description;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return file.FileName;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection, book s)
        {
            var E_name = collection["book_title"];
            var E_image = collection["image"];
            var E_price = Convert.ToDecimal(collection["price"]);
            var E_updatedate = Convert.ToDateTime(collection["update_date"]);
            var E_quantity = Convert.ToInt32(collection["quantity_in_stock"]);
            var E_descriptiomn = collection["description"];
            if (string.IsNullOrEmpty(E_name))
            {
                ViewData["Error"] = "Book Title is required";
            }
            else
            {
                s.book_title = E_name.ToString();
                s.image = E_image.ToString();
                s.price = E_price;
                s.update_date = E_updatedate;
                s.quantity_in_stock = E_quantity;
                db.books.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }

        public ActionResult Delete(int id)
        {
            var D_book = db.books.First(m => m.book_id == id);
            return View(D_book);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_book = db.books.Where(m => m.book_id == id).First();
            db.books.DeleteOnSubmit(D_book);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}
