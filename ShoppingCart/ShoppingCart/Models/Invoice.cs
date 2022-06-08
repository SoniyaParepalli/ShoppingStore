using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class Invoice
    {
        public int Invoice_Id { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public double? Invoice_TotalBill { get; set; }
        public int? UserId { get; set; }
    }
}