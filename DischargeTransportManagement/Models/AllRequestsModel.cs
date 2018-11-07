namespace DischargeTransportManagement.Models
{
    public class AllRequestsModel
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }
        public string Destination { get; set; }
        public string Agency { get; set; }
        public string ScheduledTime { get; set; }
        public string Status { get; set; }
        public string RequestType { get; set; }
        public string MRN { get; set; }
        public string Location { get; set; }
    }
}