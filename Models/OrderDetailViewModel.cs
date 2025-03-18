using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project3.Models
{
        public class OrderDetailViewModel
        {
        public int OrderID { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}