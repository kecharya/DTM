//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DischargeTransportManagement.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblDischargeATNumber
    {
        public int ATID { get; set; }
        public string ATNumber { get; set; }
        public int RequestID { get; set; }
        public string PayingReason { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Payor { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceCreatedOn { get; set; }
        public string InvoiceCreatedBy { get; set; }
        public string PONumber { get; set; }
        public Nullable<System.DateTime> POPaidOn { get; set; }
        public string POCreatedBy { get; set; }
    
        public virtual tblDischargeRequest1 tblDischargeRequest { get; set; }
    }
}