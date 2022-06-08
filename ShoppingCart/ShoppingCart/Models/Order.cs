using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class Order
    {
        public int Order_Id { get; set; }
        public DateTime? Order_Date { get; set; }
        public int? Order_Quantity { get; set; }
        public double? Order_Bill { get; set; }
        public int? Order_UnitPrice { get; set; }
        public int? Product_Id { get; set; }
        public int? Invoice_Id { get; set; }
    }
}