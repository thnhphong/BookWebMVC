using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project3.Models
{
    public class Order
    {
        public int orderID { get; set; }
        public string customerID { get; set; }
        public bool isShip { get; set; }
        public bool isPayment { get; set; }
        public DateTime deliveryDate { get; set; }
        public DateTime orderDate { get; set; }
        public decimal? total { get; set; }
    }

}