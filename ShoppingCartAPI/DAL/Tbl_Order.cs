//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Order
    {
        public int Order_Id { get; set; }
        public Nullable<System.DateTime> Order_Date { get; set; }
        public Nullable<int> Order_Quantity { get; set; }
        public Nullable<decimal> Order_Bill { get; set; }
        public Nullable<int> Order_UnitPrice { get; set; }
        public Nullable<int> Product_Id { get; set; }
        public Nullable<int> Invoice_Id { get; set; }
    }
}
