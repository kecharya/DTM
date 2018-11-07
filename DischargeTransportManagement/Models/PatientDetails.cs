namespace DischargeTransportManagement.Models
{
    public class PatientDetails
    {
        ///TODO: move to the config file
        public PatientDetails()
        {
            ApplicationName = "LifeFlightDTM";
            RequestType = "PatientInsuranceRequest";
            AppPassKeyPrd = "r#A0hl7Q";
            AppPassKeyDev = "w5T#9y3q";
            VunetId = "tatiner";
            MrnDev = "30263032";
            MrnPrd = "002506400";
        }
        public string ApplicationName { get; set; }
        public string RequestType { get; set; }
        public string AppPassKeyDev { get; set; }
        public string AppPassKeyPrd { get; set; }
        public string VunetId { get; set; }
        public string MrnDev { get; set; }
        public string MrnPrd { get; set; }
    }
}