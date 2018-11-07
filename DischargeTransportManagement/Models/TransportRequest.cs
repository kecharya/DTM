using DischargeTransportManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Models
{
    public class TransportRequestViewModel
    {
        //public IEnumerable<tblTransportMode> TransModes { get; set; }

        //public IEnumerable<tblCareLevel> CareLevels { get; set; }
        public TransportRequestViewModel()
        {

        }
        public TransportRequestViewModel(string stateSelected, string emsAgencySelected, string tansportModeSelected)
        {
            SelectedState = stateSelected;
            SelectedEmsAgency = emsAgencySelected;
            SelectedTransportMode = tansportModeSelected;
        }


        public string SelectedState { get; set; }
        public string SelectedEmsAgency { get; set; }
        public string SelectedTransportMode { get; set; }
        public IEnumerable<SelectListItem> TransportMode { get; set; }
        public IEnumerable<SelectListItem> CareLevels { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }
        public List<string> PickUpLocations
        {
            get
            {
                return new List<string> { "VUH", "VCH" };
            }
        }

        public string AgencyPhone
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedEmsAgency) || SelectedEmsAgency == "Choose the Agency")
                {
                    return string.Empty;
                }
                else
                {
                    int emsid = Convert.ToInt32(SelectedEmsAgency);
                    using (var context = new lifeflightapps())
                    {
                        var query = (from phone in context.tblEmsAgencyLocals where phone.EMSID.Equals(emsid) select phone.Phones).FirstOrDefault();
                        string abc = string.Empty;
                        abc = string.IsNullOrWhiteSpace(query) ? "N/A" : query;

                        return abc;
                    }
                    //query == (string.IsNullOrWhiteSpace(query) ? "N/A" : query);
                    //var query = (from phone in db.EmsAgencies.Where phone.
                    //             (e => e.EMSID == emsid).Select(ph => ph.Phones);
                    //return string.Empty;
                }
                //else return db.EmsAgencies.Where(e => e.EMSID == Convert.ToInt32(SelectedEmsAgency)).Select(ph => ph.Phones).ToString();
            }
        }

        public string AgencyTaxId
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedEmsAgency) || SelectedEmsAgency == "Choose the Agency")
                {
                    return string.Empty;
                }
                else
                {
                    int emsid = Convert.ToInt32(SelectedEmsAgency);
                    using (var context = new lifeflightapps())
                    {
                        var query = (from tax in context.tblEmsAgencyLocals where tax.EMSID.Equals(emsid) select tax.taxID).FirstOrDefault();
                        return query ?? "N/A";
                    }
                }
            }
        }


        public IEnumerable<SelectListItem> EmsAgencies { get; set; }

        public IEnumerable<SelectListItem> GetEmsAgencies()
        {
            //
            string ss = (!string.IsNullOrEmpty(SelectedState)) ? SelectedState : "TN";
            string stm = (!string.IsNullOrEmpty(SelectedTransportMode)) ? SelectedTransportMode : "Ground EMS";
            using (var context = new lifeflightapps())
            {
                return context.tblEmsAgencyLocals.Where(ems => ems.State == ss && ems.Type == stm).Select(e => new SelectListItem()
                {
                    Value = e.EMSID.ToString(),
                    Text = e.Name,
                    Selected = (e.EMSID.ToString() == SelectedEmsAgency)
                });
            }
            /*
            if ((string.IsNullOrEmpty(SelectedState)) && (string.IsNullOrEmpty(SelectedTransportMode)))
            {
                List<SelectListItem> emptylist = new List<SelectListItem>();
                 emptylist.Insert(0,new SelectListItem { Value = string.Empty, Text="State Needs to be Selected"});
                //return emptylist;
                //.Insert(0, new SelectListItem { Text = "Select State", Value = string.Empty});

                return db.EmsAgencies.Where(ems => ems.State == ((!string.IsNullOrEmpty(SelectedState)) ? SelectedState : "TN")).Select(e => new SelectListItem()
                {
                    Value = e.EMSID.ToString(),
                    Text = e.Name,

                });

            }
            return db.EmsAgencies.Where(ems => ems.State == SelectedState  && ems.Type == SelectedTransportMode).Select(e => new SelectListItem()
            {
                Value = e.EMSID.ToString(),
                Text = e.Name.ToString(),
                Selected = (e.EMSID.ToString() == SelectedEmsAgency)                
            });
            */
        }

        public IEnumerable<SelectListItem> GetTransportModes()
        {
            //if (string.IsNullOrEmpty(SelectedTransportMode)
            //{

            //}
            using (var context = new lifeflightapps())
            {
                return context.tblTransportModes.Select(t => new SelectListItem()
                {
                    Text = t.TransportMode,
                    Value = t.TransportMode,
                    Selected = (t.TransportMode == SelectedTransportMode)
                });
            }
        }

        public IEnumerable<SelectListItem> GetCareLevels()
        {
            using (var context = new lifeflightapps())
            {
                return context.tblCareLevels.Select(c => new SelectListItem()
                {
                    Text = c.LevelOfCare,
                    Value = c.carelevelID.ToString()
                });
            }
        }

        public IEnumerable<SelectListItem> GetStates()
        {
            string sst = (!string.IsNullOrEmpty(SelectedState)) ? SelectedState : "TN";
            using (var context = new lifeflightapps())
            {
                return context.tblEmsAgencyLocals.Where(st => st.State != null).Select(s => new SelectListItem()
                {
                    Value = s.State,
                    Text = s.State,
                    Selected = (s.State == ((!string.IsNullOrEmpty(SelectedState)) ? SelectedState : "TN"))
                }).Distinct();
            }
        }

    }
}