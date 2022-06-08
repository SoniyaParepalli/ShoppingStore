using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class Cart
    {
        public int Productid { get; set; }
        public string Productname { get; set; }
        public float Price { get; set; }
        public int Qty { get; set; }
        public float Bill { get; set; }
    }
}