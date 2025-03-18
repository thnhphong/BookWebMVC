using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Project3.Models
{
    public class Cart
    {
        dbBookstoreDataContext db = new dbBookstoreDataContext(ConfigurationManager.ConnectionStrings["BookStoreConnectionString8"].ConnectionString);

        public int book_id { get; set; }

        [Display(Name = "Book name")]
        public string book_name { get; set; }

        [Display(Name = "Cover image")]
        public string image { get; set; }

        [Display(Name = "Price")]
        public decimal price { get; set; }

        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        [Display(Name = "Total")]
        public decimal Total
        {
            get { return quantity * price; }
        }

        public Cart(int id)
        {
            book_id = id;
            book b = db.books.Single(n => n.book_id == book_id);
            book_name = b.book_title;
            image = b.image;
            price = decimal.Parse(b.price.ToString());
            quantity = 1;
        }
    }

}