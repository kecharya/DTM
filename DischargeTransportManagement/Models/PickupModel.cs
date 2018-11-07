namespace DischargeTransportManagement.Models
{
    public class PickupModel
    {
        public int PickupId { get; set; }
        public string PickupLocationName  { get; set; }
        public string PickupInstructions { get; set; }
        public string AddressLineOne { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

    }
}