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
    
    public partial class tblConfirmationEmailEM
    {
        public int emsemailID { get; set; }
        public Nullable<int> reqID { get; set; }
        public Nullable<System.DateTime> emailedon { get; set; }
        public string emailedagency { get; set; }
        public string emailaddress { get; set; }
        public string emailedby { get; set; }
    }
}