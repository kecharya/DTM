namespace DischargeTransportManagement.Models
{
    public class Patient
    {
        public int? PatientId { get; set; }
        public string MRN { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DOB { get; set; }
        public int? Age { get; set; }
        public string Weight { get; set; }
        public string Social { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string Phone { get; set; }
        public string WeightUnit { get; set; }

    }
}