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
    
    public partial class tblDischargeRequest1
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDischargeRequest1()
        {
            this.tblDischargeATNumbers = new HashSet<tblDischargeATNumber>();
            this.tblPageLogs = new HashSet<tblPageLog>();
        }
    
        public int RequestID { get; set; }
        public Nullable<int> CallerID { get; set; }
        public string MrNumber { get; set; }
        public int DestinationID { get; set; }
        public string ModeOfTransport { get; set; }
        public string SpecialInstructions { get; set; }
        public Nullable<System.DateTime> DischargeTime { get; set; }
        public int InsuranceID { get; set; }
        public int PatientID { get; set; }
        public int LocationID { get; set; }
        public int RequestStatusID { get; set; }
        public string CallReceivedBy { get; set; }
        public Nullable<System.DateTime> CallReceivedDate { get; set; }
        public string LifeSupport { get; set; }
        public Nullable<int> EmsAgencyId { get; set; }
        public Nullable<int> PickupID { get; set; }
        public string RequestType { get; set; }
        public Nullable<System.DateTime> EMSAgencyContacted { get; set; }
        public Nullable<System.DateTime> EMSAgencyResponded { get; set; }
        public Nullable<System.DateTime> EMSAgencyArrived { get; set; }
        public string CaseNumber { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> AppointmentTime { get; set; }
        public Nullable<int> EmsUnitId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDischargeATNumber> tblDischargeATNumbers { get; set; }
        public virtual tblMbrCensu tblMbrCensu { get; set; }
        public virtual tblMbrInsurance tblMbrInsurance { get; set; }
        public virtual tblPatient tblPatient { get; set; }
        public virtual tblRequestStatu tblRequestStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPageLog> tblPageLogs { get; set; }
        public virtual tblEmsAgencyLocal tblEmsAgencyLocal { get; set; }
        public virtual tblDischargeDestination tblDischargeDestination { get; set; }
    }
}