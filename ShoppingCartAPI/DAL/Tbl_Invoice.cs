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
    
    public partial class Tbl_Invoice
    {
        public int Invoice_Id { get; set; }
        public Nullable<System.DateTime> Invoice_Date { get; set; }
        public Nullable<decimal> Invoice_TotalBill { get; set; }
        public Nullable<int> UserId { get; set; }
    }
}
