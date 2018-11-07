using System.Data.Entity;

namespace DischargeTransportManagement.Domain.DAL
{
    public class DischargeTransportManagementContext : DbContext
    {
        public DischargeTransportManagementContext() : base("lifeflightapps")
        {
        }
        public DbSet<tblCaller> Callers { get; set; }
        public DbSet<tblTransportMode> ModesOfTransport { get; set; }
        public DbSet<tblCareLevel> CareLevel { get; set; }
        public DbSet<tblEmsAgencyLocal> EmsAgencies { get; set; }
        public DbSet<tblNursingHome> NursingHomes { get; set; }
        public DbSet<tblHospital> Hospitals { get; set; }
    }
}
