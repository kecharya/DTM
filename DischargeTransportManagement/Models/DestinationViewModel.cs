using DischargeTransportManagement.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DischargeTransportManagement.Models
{
    public class DestinationViewModel
    {
        
        List<Destination> destinationList = new List<Destination>();
        /// <summary>
        /// Gets all the destinations
        /// </summary>
        /// <returns></returns>
        public List<Destination> RetrieveDestinations()
        {
            using (var context = new lifeflightapps())
            {
                //retrieve the nursinghomes
                var query1 = context.tblNursingHomes.Select(n => new { n.NursingHomeID, n.Name, n.Address, n.City, n.State, n.Zip, n.County, n.Phone }).AsEnumerable();
                //retrieve the hospitals
                var query2 = context.tblHospitals.Select(h => new { h.ID2, h.Name, h.Address, h.City, h.State, h.Zip, h.Country }).Where(a => a.ID2 < 6231).AsEnumerable();

                //add the nursinghomes to destinationlist
                foreach (var nursingHome in query1)
                {
                    Destination nursingDestination = new Destination()
                    {
                        DestinationId = nursingHome.NursingHomeID,
                        DestinationName = nursingHome.Name,
                        Address = nursingHome.Address,
                        City = nursingHome.City,
                        County = nursingHome.County,
                        State = nursingHome.State,
                        Zip = nursingHome.Zip.ToString(),
                        Phone = nursingHome.Phone
                    };
                    destinationList.Add(nursingDestination);
                }

                //add the hospitals to destinationlist
                foreach (var hospitals in query2)
                {
                    Destination hospitalDestination = new Destination()
                    {
                        DestinationId = hospitals.ID2,
                        DestinationName = hospitals.Name,
                        Address = hospitals.Address,
                        City = hospitals.City,
                        County = hospitals.Country,
                        State = hospitals.State,
                        Zip = hospitals.Zip,
                        Phone = string.Empty
                    };

                    destinationList.Add(hospitalDestination);
                }
            }
            return destinationList;
        }

        /// <summary>
        /// Gets the filtered destinations
        /// </summary>
        /// <param name="destinationName"></param>
        /// <returns></returns>
        public List<Destination> RetrieveDesiredDestinations(string destinationName)
        {
            using (var context = new lifeflightapps())
            {
                //retrieve the nursinghomes
                var query1 = context.tblNursingHomes.Select(n => new { n.NursingHomeID, n.Name, n.Address, n.City, n.State, n.Zip, n.County, n.Phone }).Where(nu => nu.Name.StartsWith(destinationName)).AsEnumerable();
                //retrieve the hospitals
                var query2 = context.tblHospitals.Select(h => new { h.ID2, h.Name, h.Address, h.City, h.State, h.Zip, h.County }).Where(a => a.ID2 < 6231 && a.Name.StartsWith(destinationName)).AsEnumerable();

                //add the nursinghomes to destinationlist
                foreach (var nursingHome in query1)
                {
                    Destination nursingDestination = new Destination()
                    {
                        DestinationId = nursingHome.NursingHomeID,
                        DestinationName = nursingHome.Name,
                        Address = nursingHome.Address,
                        City = nursingHome.City,
                        County = nursingHome.County,
                        State = nursingHome.State,
                        Zip = nursingHome.Zip.ToString(),
                        Phone = nursingHome.Phone
                    };
                    destinationList.Add(nursingDestination);
                }

                //add the hospitals to destinationlist
                foreach (var hospitals in query2)
                {
                    Destination hospitalDestination = new Destination()
                    {
                        DestinationId = hospitals.ID2,
                        DestinationName = hospitals.Name,
                        Address = hospitals.Address,
                        City = hospitals.City,
                        County = hospitals.County,
                        State = hospitals.State,
                        Zip = hospitals.Zip,
                        Phone = string.Empty
                    };

                    destinationList.Add(hospitalDestination);
                }
            }
            return destinationList;
        }
    }
}