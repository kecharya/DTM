using DischargeTransportManagement.Domain;
using DischargeTransportManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DischargeTransportManagement.Controllers
{
    public class AuditLogController : Controller
    {

        // GET: AuditLog
        public ActionResult RequestHistory(int? RequestId)
        {
            List<AuditLog> logs = new List<AuditLog>();
            List<tblAuditLog> tblAuditLogs = new List<tblAuditLog>();
            
            using (var context = new lifeflightapps())
            {
                tblAuditLogs = context.tblAuditLogs.Where(x => x.IDFROMAPP == RequestId).OrderBy(x=>x.CREATEDON).ToList();
            }
            if (tblAuditLogs.Count>0)
            {
                logs = ConvertToAuditLogs(tblAuditLogs);
            }
            return View(logs);
        }

        /// <summary>
        /// Converts the entity to Model
        /// </summary>
        /// <param name="tblAuditLogs"></param>
        /// <returns></returns>
        private List<AuditLog> ConvertToAuditLogs(List<tblAuditLog> tblAuditLogs)
        {
            List<AuditLog> logs = new List<AuditLog>();
            
            foreach (var log in tblAuditLogs)
            {
                AuditLog auditLog = new AuditLog()
                {
                    RequestId = log.IDFROMAPP,
                    MrNumber = log.MRNUMBER,
                    Notes = log.UPDATENOTES,
                    CreatedBy = log.CREATEDBY,
                    CreatedOn = log.CREATEDON
                };
                logs.Add(auditLog);
            }
            return logs;
            
        }
    }
}