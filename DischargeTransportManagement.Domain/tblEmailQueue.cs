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
    
    public partial class tblEmailQueue
    {
        public int emailID { get; set; }
        public string emailaddress { get; set; }
        public string emailtype { get; set; }
        public string emailbody { get; set; }
        public string emailattachment { get; set; }
        public Nullable<System.DateTime> emailsent { get; set; }
        public Nullable<System.DateTime> emailqueued { get; set; }
        public int ReqID { get; set; }
        public string patientlastname { get; set; }
    }
}
