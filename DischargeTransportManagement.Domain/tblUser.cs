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
    
    public partial class tblUser
    {
        public Nullable<double> ID { get; set; }
        public string VUNetID { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public Nullable<int> DTMLevel { get; set; }
        public Nullable<int> VACLevel { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
    }
}
