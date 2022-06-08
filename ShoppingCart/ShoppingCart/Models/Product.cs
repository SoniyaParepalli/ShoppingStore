using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class Product
    {
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public double? Product_Price { get; set; }
        public string Product_Desc { get; set; }
        public string Product_Image { get; set; }
    }
}