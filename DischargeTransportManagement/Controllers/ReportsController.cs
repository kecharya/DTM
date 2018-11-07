using Dapper;
using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models.ReportModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Configuration;

namespace DischargeTransportManagement.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult AllReports()
        {
            return View();
        }

        public ActionResult Transports(int? page, DateTime? start, DateTime? end)
        {
            List<RequestsPerMonth> requestsPerMonths = new List<RequestsPerMonth>();
            requestsPerMonths = GetRequests(start, end);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(requestsPerMonths.OrderBy(x => x.RequestDateTime).ToPagedList(pageNumber, pageSize));

        }

        public ActionResult AtTransports(int? page, DateTime? start, DateTime? end)
        {
            List<ATTransport> atTransports = new List<ATTransport>();
            atTransports = GetATTransports(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(atTransports.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VanTransports(int? page, DateTime? start, DateTime? end)
        {
            List<VanTransport> atTransports = new List<VanTransport>();
            atTransports = GetVanTransports(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(atTransports.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult StallworthTransports(int? page, DateTime? start, DateTime? end)
        {
            List<StallworthAtNumbers> stallworths = new List<StallworthAtNumbers>();

            var stallworthAts = GetStallworthAtNumbers(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(stallworthAts.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult VanderbiltEMS(int? page, DateTime? start, DateTime? end)
        {
            List<VanderbiltEMS> VandyEmsTransports = new List<VanderbiltEMS>();
            VandyEmsTransports = GetVandyEmsTransports(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(VandyEmsTransports.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VCHTransports(int? page, DateTime? start, DateTime? end)
        {
            List<VCHTransport> VchTransports = new List<VCHTransport>();
            VchTransports = GetVchTransports(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(VchTransports.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VumcAtNumbers(int? page, DateTime? start, DateTime? end)
        {
            List<VumcAtNumbers> vumcAtNumbers = new List<VumcAtNumbers>();
            vumcAtNumbers = GetVumcAtNumbers(start, end);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(vumcAtNumbers.OrderBy(x => x.PatientPickupTime).ToPagedList(pageNumber, pageSize));
        }



        private List<RequestsPerMonth> GetRequests(DateTime? start, DateTime? end)
        {
            List<RequestsPerMonth> requests = new List<RequestsPerMonth>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);
            using (var context = new lifeflightapps())
            {
                
                
                var req = (from r in context.tblDischargeRequests1
                           join c in context.tblMbrCensus
                           on r.LocationID equals c.MbrCensusID
                           join d in context.tblDischargeDestinations
                           on r.DestinationID equals d.DestinationID
                           join p in context.tblPatients
                           on r.PatientID equals p.PatientID
                           join e in context.tblEmsAgencyLocals
                           on r.EmsAgencyId equals e.EMSID
                           //into q from qr in q 
                           where
                           r.RequestStatusID == 1 &&
                           r.DischargeTime.Value > firstDayOfMonth &&
                           r.DischargeTime.Value < lastDayOfMonth
                           select new { r.DischargeTime, r.RequestType, e.Name, p.FirstName, p.LastName, p.MrNumber, c.PavillionCode, c.Unit, c.Bed, d.DestinationName, d.DestinationType, d.AddressLineOne, d.City, d.StateCode, d.Zip }
                          ).ToList();

                foreach (var item in req)
                {
                    RequestsPerMonth rpm = new RequestsPerMonth()
                    {
                        RequestDateTime = item.DischargeTime,
                        Type = item.RequestType,
                        Patient = item.LastName + " " + item.MrNumber,
                        EmsAgency = item.Name,
                        Location = item.PavillionCode + " - " + item.Unit + " - " + item.Bed,
                        Destination = item.AddressLineOne + "," + item.City + "," + item.StateCode + " " + item.Zip

                    };
                    requests.Add(rpm);
                }

            }
            return requests;
        }



        private List<ATTransport> GetATTransports(DateTime? start, DateTime? end)
        {
            List<ATTransport> aTTransports = new List<ATTransport>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);

            string sql = "select PatientPickupTime, Type, Patient, FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EMSAgency, Miles, Cost, AtNumber, Payor, Reason, RequestStatus from DTM.SSRS_Data where ATNUMBER is not null and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var atTransport = con.Query<ATTransport>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                aTTransports = atTransport;

            }
            return aTTransports;
        }

        private List<VanTransport> GetVanTransports(DateTime? start, DateTime? end)
        {
            List<VanTransport> vanTransports = new List<VanTransport>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);

            string sql = "select PatientPickupTime, CallerName, CallerTitle, RequestStatus, FromFacility +' '+ FromUnit +' '+FromRoom as Origin,Destination, Miles, EmsAgency, Patient, Patient + ' ' + PatientMRN as PatientWithMRN, PatientMRN from DTM.SSRS_Data where EmsAgency in ('LifeFlight Van', 'PAS Van') AND PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            //select PatientPickupTime, Type, Patient, FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EMSAgency, Miles, Cost, AtNumber, Payor, Reason, RequestStatus from DTM.SSRS_Data where ATNUMBER is not null and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var vanTransport = con.Query<VanTransport>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                vanTransports = vanTransport;

            }

            return vanTransports;
        }

        private List<StallworthAtNumbers> GetStallworthAtNumbers(DateTime? start, DateTime? end)
        {
            List<StallworthAtNumbers> stallworths = new List<StallworthAtNumbers>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);
            
            string sql = "select PatientPickupTime, Type, Patient, FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EMSAgency, Miles, Cost, AtNumber, Payor, Reason from DTM.SSRS_Data where Payor = 'Stallworth' and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            //SqlConnection oConn = new SqlConnection(ConfigurationManager.ConnectionStrings["lifeflightapps"].ConnectionString);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var stallworth = con.Query<StallworthAtNumbers>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                stallworths = stallworth;

            }
            return stallworths;
        }

        private List<VanderbiltEMS> GetVandyEmsTransports(DateTime? start, DateTime? end)
        {
            List<VanderbiltEMS> VandyEms = new List<VanderbiltEMS>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);

            string sql = "select PatientPickupTime, CallerName, CallerTitle, RequestStatus, Type,  FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EmsAgency,Patient, PatientMRN from DTM.SSRS_Data where EMSAgency='Vanderbilt EMS' and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var vandyEms = con.Query<VanderbiltEMS>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                VandyEms = vandyEms;

            }
            return VandyEms;
        }

        private List<VCHTransport> GetVchTransports(DateTime? start, DateTime? end)
        {
            List<VCHTransport> VchTransports = new List<VCHTransport>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);

            string sql = "select PatientPickupTime, CallerName, CallerTitle, RequestStatus, Type,  FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EmsAgency,Patient, PatientMRN from DTM.SSRS_Data where FromFacility  in ('VCH', 'Vanderbilt Children''s', 'Vanderbilt Childrens Hospital' ) and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var vhcTransport = con.Query<VCHTransport>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                VchTransports = vhcTransport;

            }
            return VchTransports;
        }

        private List<VumcAtNumbers> GetVumcAtNumbers(DateTime? start, DateTime? end)
        {
            List<VumcAtNumbers> vumcAtNumbers = new List<VumcAtNumbers>();
            DateTime firstDayOfMonth, lastDayOfMonth;
            GetDates(start, end, out firstDayOfMonth, out lastDayOfMonth);

            string sql = "select PatientPickupTime,  Type, Patient, FromFacility +' '+ FromUnit +' '+FromRoom as Origin, Destination, EmsAgency, miles, cost, atnumber, payor, Reason from DTM.SSRS_Data where Payor = 'VUMC' and EmsAgency = 'Vanderbilt EMS' and PatientPickupTime > @firstDayOfMonth and  PatientPickupTime < @lastDayOfMonth";
            var parameters = new List<DynamicParameters>();
            var param = new DynamicParameters();
            param.Add("@firstDayOfMonth", firstDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("@lastDayOfMonth", lastDayOfMonth, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameters.Add(param);
            var connection = ConfigurationManager.ConnectionStrings["lfDapper"].ConnectionString;
            using (var con = new SqlConnection(connection))
            {

                var vumcAts = con.Query<VumcAtNumbers>(sql, param, commandType: System.Data.CommandType.Text).ToList();

                vumcAtNumbers = vumcAts;

            }
            return vumcAtNumbers;
        }

        private static void GetDates(DateTime? start, DateTime? end, out DateTime firstDayOfMonth, out DateTime lastDayOfMonth)
        {
            DateTime date;
            if ((!start.HasValue) && (!end.HasValue))
            {
                date = DateTime.Today;
                firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            }
            else
            {
                firstDayOfMonth = start.Value; 
                lastDayOfMonth = end.Value;  
            }
        }

    }
}