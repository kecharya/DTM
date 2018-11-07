using DischargeTransportManagement.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class RequestModel
    {
        public string CallerName { get; set; }
        public string MrNumber { get; set; }
        public string Destination { get; set; }
        public List<SelectListItem> EmsAgency { get; set; }
        //[Required(ErrorMessage = "Mode Needed")]
        public string SelectedEmsAgency { get; set; }
        public string SpecialInstructions { get; set; }
        public string DischargeTime { get; set; }
        public List<SelectListItem> SpecialNeeds { get; set; }
        
        public string SelectedSpecialNeed { get; set; }
        //[Required(ErrorMessage = "Destination Type Needed")]
        public string DestinationType { get; set; }
        public string DestinationName { get; set; }
        public string AddressLineOne { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string LifeSupport { get; set; }
        public int PatientWeight { get; set; }
        public string PickupName { get; set; }
        public string PickupInstructions { get; set; }
        public string PickupAddressLine1 { get; set; }
        public string PickupCity { get; set; }
        public string PickupState { get; set; }
        public string PickupZip { get; set; }
        public string Phone { get; set; }
        public string DestinationInstructions { get; set; }
        public string RequestType { get; set; }
        public string RequesStatus { get; set; }
        public string PatientName { get; set; }
        public string PatientInsuranceId { get; set; }
        public string PatientInsuranceProvider { get; set; }
        public string IsPostSuccess { get; set; }
        public string WeightUnit { get; set; }
        public string Miles { get; set; }
        public string TravelTime { get; set; }
        public string Notes { get; set; }
        public string AppointmentTime { get; set; }

        public IEnumerable<SelectListItem> GetStatesList()
        {
            IList<SelectListItem> states = new List<SelectListItem>
        {
            new SelectListItem() {Text="", Value=""},
            new SelectListItem() {Text="Alabama", Value="AL"},
            new SelectListItem() { Text="Alaska", Value="AK"},
            new SelectListItem() { Text="Arizona", Value="AZ"},
            new SelectListItem() { Text="Arkansas", Value="AR"},
            new SelectListItem() { Text="California", Value="CA"},
            new SelectListItem() { Text="Colorado", Value="CO"},
            new SelectListItem() { Text="Connecticut", Value="CT"},
            new SelectListItem() { Text="District of Columbia", Value="DC"},
            new SelectListItem() { Text="Delaware", Value="DE"},
            new SelectListItem() { Text="Florida", Value="FL"},
            new SelectListItem() { Text="Georgia", Value="GA"},
            new SelectListItem() { Text="Hawaii", Value="HI"},
            new SelectListItem() { Text="Idaho", Value="ID"},
            new SelectListItem() { Text="Illinois", Value="IL"},
            new SelectListItem() { Text="Indiana", Value="IN"},
            new SelectListItem() { Text="Iowa", Value="IA"},
            new SelectListItem() { Text="Kansas", Value="KS"},
            new SelectListItem() { Text="Kentucky", Value="KY"},
            new SelectListItem() { Text="Louisiana", Value="LA"},
            new SelectListItem() { Text="Maine", Value="ME"},
            new SelectListItem() { Text="Maryland", Value="MD"},
            new SelectListItem() { Text="Massachusetts", Value="MA"},
            new SelectListItem() { Text="Michigan", Value="MI"},
            new SelectListItem() { Text="Minnesota", Value="MN"},
            new SelectListItem() { Text="Mississippi", Value="MS"},
            new SelectListItem() { Text="Missouri", Value="MO"},
            new SelectListItem() { Text="Montana", Value="MT"},
            new SelectListItem() { Text="Nebraska", Value="NE"},
            new SelectListItem() { Text="Nevada", Value="NV"},
            new SelectListItem() { Text="New Hampshire", Value="NH"},
            new SelectListItem() { Text="New Jersey", Value="NJ"},
            new SelectListItem() { Text="New Mexico", Value="NM"},
            new SelectListItem() { Text="New York", Value="NY"},
            new SelectListItem() { Text="North Carolina", Value="NC"},
            new SelectListItem() { Text="North Dakota", Value="ND"},
            new SelectListItem() { Text="Ohio", Value="OH"},
            new SelectListItem() { Text="Oklahoma", Value="OK"},
            new SelectListItem() { Text="Oregon", Value="OR"},
            new SelectListItem() { Text="Pennsylvania", Value="PA"},
            new SelectListItem() { Text="Puerto Rico", Value="PR"},
            new SelectListItem() { Text="Rhode Island", Value="RI"},
            new SelectListItem() { Text="South Carolina", Value="SC"},
            new SelectListItem() { Text="South Dakota", Value="SD"},
            new SelectListItem() { Text="Tennessee", Value="TN", Selected=true},
            new SelectListItem() { Text="Texas", Value="TX"},
            new SelectListItem() { Text="Utah", Value="UT"},
            new SelectListItem() { Text="Vermont", Value="VT"},
            new SelectListItem() { Text="Virginia", Value="VA"},
            new SelectListItem() { Text="Washington", Value="WA"},
            new SelectListItem() { Text="West Virginia", Value="WV"},
            new SelectListItem() { Text="Wisconsin", Value="WI"},
            new SelectListItem() { Text="Wyoming", Value="WY"}
        };
            return states;
        }

        public IEnumerable<SelectListItem> GetEmsAgencies()
        {
            IList<SelectListItem> emsAgencies = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps() )
            {
                var ems = context.tblEmsAgencyLocals.Where(x => x.LocalUse == true).Select(e => new SelectListItem()
                {
                    Value = e.Name,
                    Text = e.Name
                });
                emsAgencies = ems.ToList();
            }
            emsAgencies.Add(new SelectListItem { Text = "-Choose Agency-", Value = "-Choose Agency-", Selected = true });
            return emsAgencies;
        }

        public IEnumerable<SelectListItem> GetModesOfTransport()
        {
            IList<SelectListItem> modesOfTransport = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                var ems = context.tblTransportModes.Select(e => new SelectListItem()
                {
                    Value = e.TransportMode,
                    Text = e.TransportMode
                });
                modesOfTransport = ems.ToList();
            }
            return modesOfTransport;
        }

        public IEnumerable<SelectListItem> GetStatuses()
        {
            IList<SelectListItem> statuses = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                var statusFromDb = context.tblRequestStatus.Select(e => new SelectListItem()
                {
                    Value = e.ReqStatusID.ToString(),
                    Text = e.RequestStatus
                });
                statuses = statusFromDb.ToList();
            }
            return statuses;
        }
        public IEnumerable<SelectListItem> GetSpecialNeeds()
        {
            IList<SelectListItem> SpecialNeeds = new List<SelectListItem>();
            using (lifeflightapps context = new lifeflightapps())
            {
                SpecialNeeds = context.tblSpecialNeeds.Select(s => new SelectListItem()
                {
                    Text = s.SpecialNeed,
                    Value = s.SpecialNeedID.ToString()
                }).ToList();
            }
            return SpecialNeeds;
        }
    }
}