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
    
    public partial class tblDischargeRequestLog
    {
        public int ReqLogID { get; set; }
        public Nullable<int> ReqID { get; set; }
        public string EnteredBy { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public string Notes { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
    }
}
