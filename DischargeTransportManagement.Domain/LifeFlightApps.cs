namespace DTM.Domain
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LifeFlightApps : DbContext
    {
        public LifeFlightApps()
            : base("name=LifeFlightApps")
        {
        }

        public virtual DbSet<tblAmerigroupEmail> tblAmerigroupEmails { get; set; }
        public virtual DbSet<tblBedStatusUpdate> tblBedStatusUpdates { get; set; }
        public virtual DbSet<tblCaller> tblCallers { get; set; }
        public virtual DbSet<tblCallerTitle> tblCallerTitles { get; set; }
        public virtual DbSet<tblCareLevel> tblCareLevels { get; set; }
        public virtual DbSet<tblCareLevelChange> tblCareLevelChanges { get; set; }
        public virtual DbSet<tblChargeRNPageOnOff> tblChargeRNPageOnOffs { get; set; }
        public virtual DbSet<tblConfirmationEmailEM> tblConfirmationEmailEMS { get; set; }
        public virtual DbSet<tblConfirmationPage> tblConfirmationPages { get; set; }
        public virtual DbSet<tblDelayReason> tblDelayReasons { get; set; }
        public virtual DbSet<tblDeleteRecord> tblDeleteRecords { get; set; }
        public virtual DbSet<tblDischargeComplaint> tblDischargeComplaints { get; set; }
        public virtual DbSet<tblDischargeComplaintLog> tblDischargeComplaintLogs { get; set; }
        public virtual DbSet<tblDischargeComplaintType> tblDischargeComplaintTypes { get; set; }
        public virtual DbSet<tblDischargeRequest> tblDischargeRequests { get; set; }
        public virtual DbSet<tblDischargeRequestLog> tblDischargeRequestLogs { get; set; }
        public virtual DbSet<tblDischargeRequestPageLog> tblDischargeRequestPageLogs { get; set; }
        public virtual DbSet<tblDischargeRequestPhoneNumber> tblDischargeRequestPhoneNumbers { get; set; }
        public virtual DbSet<tblDischargeRequestUnit> tblDischargeRequestUnits { get; set; }
        public virtual DbSet<tblDrivingInfo> tblDrivingInfoes { get; set; }
        public virtual DbSet<tblEmailAddress> tblEmailAddresses { get; set; }
        public virtual DbSet<tblEmailQueue> tblEmailQueues { get; set; }
        public virtual DbSet<tblEmsAgencyLocal> tblEmsAgencyLocals { get; set; }
        public virtual DbSet<tblEMSAgencyRate> tblEMSAgencyRates { get; set; }
        public virtual DbSet<tblEMSArrivedBedsideLog> tblEMSArrivedBedsideLogs { get; set; }
        public virtual DbSet<tblInsuranceCompany> tblInsuranceCompanies { get; set; }
        public virtual DbSet<tblInsuranceCompanyContact> tblInsuranceCompanyContacts { get; set; }
        public virtual DbSet<tblLogonEvent> tblLogonEvents { get; set; }
        public virtual DbSet<tblMedicareCode> tblMedicareCodes { get; set; }
        public virtual DbSet<tblNursingHome> tblNursingHomes { get; set; }
        public virtual DbSet<tblPatientNote> tblPatientNotes { get; set; }
        public virtual DbSet<tblPrintFaxLog> tblPrintFaxLogs { get; set; }
        public virtual DbSet<tblQuote> tblQuotes { get; set; }
        public virtual DbSet<tblQuotesTransfer> tblQuotesTransfers { get; set; }
        public virtual DbSet<tblRateConversion> tblRateConversions { get; set; }
        public virtual DbSet<tblRecordOpened> tblRecordOpeneds { get; set; }
        public virtual DbSet<tblRecordOpenedTransfer> tblRecordOpenedTransfers { get; set; }
        public virtual DbSet<tblRequestStatu> tblRequestStatus { get; set; }
        public virtual DbSet<tblTransferRequest> tblTransferRequests { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblVUMCPayorReason> tblVUMCPayorReasons { get; set; }
        //public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<fw_ems> fw_ems { get; set; }
        public virtual DbSet<fw_log> fw_log { get; set; }
        public virtual DbSet<tblAirportNew> tblAirportNews { get; set; }
        public virtual DbSet<tblBillingStatusLog> tblBillingStatusLogs { get; set; }
        public virtual DbSet<tblCaseManager> tblCaseManagers { get; set; }
        public virtual DbSet<tblEmailQueue1> tblEmailQueue1 { get; set; }
        public virtual DbSet<tblFacility> tblFacilities { get; set; }
        public virtual DbSet<tblFacilityAirportAudit> tblFacilityAirportAudits { get; set; }
        public virtual DbSet<tblFW> tblFWs { get; set; }
        public virtual DbSet<tblFWpatient> tblFWpatients { get; set; }
        public virtual DbSet<tblHowHeard> tblHowHeards { get; set; }
        public virtual DbSet<tblInsContact> tblInsContacts { get; set; }
        public virtual DbSet<tblInsurance> tblInsurances { get; set; }
        public virtual DbSet<tblNoAngelUsedReason> tblNoAngelUsedReasons { get; set; }
        public virtual DbSet<tblPtType> tblPtTypes { get; set; }
        public virtual DbSet<tblStatu> tblStatus { get; set; }
        public virtual DbSet<EmailQueue> EmailQueues { get; set; }
        public virtual DbSet<tblAccessLog> tblAccessLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblAmerigroupEmail>()
                .Property(e => e.EmailedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblBedStatusUpdate>()
                .Property(e => e.UpdatedWhen)
                .HasPrecision(0);

            modelBuilder.Entity<tblCaller>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblCareLevel>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblCareLevelChange>()
                .Property(e => e.datetimeentered)
                .HasPrecision(0);

            modelBuilder.Entity<tblCareLevelChange>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblChargeRNPageOnOff>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblConfirmationEmailEM>()
                .Property(e => e.emailedon)
                .HasPrecision(0);

            modelBuilder.Entity<tblConfirmationPage>()
                .Property(e => e.senton)
                .HasPrecision(0);

            modelBuilder.Entity<tblDelayReason>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeComplaint>()
                .Property(e => e.ComplaintEntereddatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeComplaint>()
                .Property(e => e.ResolvedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeComplaint>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeComplaintLog>()
                .Property(e => e.LogDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeComplaintLog>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeComplaintType>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.ReqDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.PickUpDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.PickupDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.PickupTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.EMSAgencyAssignedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.EMSAgencyReqDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.EMSAgencyBedSide)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.PatientDOB)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.AuthorizationRx)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.BedStatusUpdated)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequest>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeRequestLog>()
                .Property(e => e.LogDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblDischargeRequestLog>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDischargeRequestPageLog>()
                .Property(e => e.datetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblEmailQueue>()
                .Property(e => e.emailsent)
                .HasPrecision(0);

            modelBuilder.Entity<tblEmailQueue>()
                .Property(e => e.emailqueued)
                .HasPrecision(0);

            modelBuilder.Entity<tblEmsAgencyLocal>()
                .Property(e => e.BaseRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEmsAgencyLocal>()
                .Property(e => e.MileageRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEmsAgencyLocal>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0425)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0426)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0427)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0428)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0429)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0430)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0434)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSAgencyRate>()
                .Property(e => e.A0435)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblEMSArrivedBedsideLog>()
                .Property(e => e.sentondatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblInsuranceCompany>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblInsuranceCompanyContact>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblLogonEvent>()
                .Property(e => e.logondatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblMedicareCode>()
                .Property(e => e.MedicareAllowable)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblMedicareCode>()
                .Property(e => e.VUMCRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblNursingHome>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPatientNote>()
                .Property(e => e.UpdatedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblPatientNote>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPrintFaxLog>()
                .Property(e => e.printeddatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblQuote>()
                .Property(e => e.Quote)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblQuote>()
                .Property(e => e.QuoteDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblQuotesTransfer>()
                .Property(e => e.Quote)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblQuotesTransfer>()
                .Property(e => e.QuoteDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblRecordOpened>()
                .Property(e => e.opendatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblRecordOpenedTransfer>()
                .Property(e => e.opendatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.ReqDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.PickUpDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.PickupDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.PickupTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.EMSAgencyAssignedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.EMSAgencyReqDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.EMSAgencyBedSide)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.PatientDOB)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.AuthorizationRx)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.ApptDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.ApptDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.ApptTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblTransferRequest>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblUser>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblVUMCPayorReason>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            //modelBuilder.Entity<Audit>()
            //    .Property(e => e.EditDate)
            //    .HasPrecision(0);

            //modelBuilder.Entity<Audit>()
            //    .Property(e => e.SSMA_TimeStamp)
            //    .IsFixedLength();

            modelBuilder.Entity<fw_ems>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<fw_log>()
                .Property(e => e.date)
                .HasPrecision(0);

            modelBuilder.Entity<fw_log>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblAirportNew>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblBillingStatusLog>()
                .Property(e => e.eventdatetime)
                .HasPrecision(0);

            modelBuilder.Entity<tblCaseManager>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblEmailQueue1>()
                .Property(e => e.emailsent)
                .HasPrecision(0);

            modelBuilder.Entity<tblEmailQueue1>()
                .Property(e => e.emailqueued)
                .HasPrecision(0);

            modelBuilder.Entity<tblFacility>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblFacilityAirportAudit>()
                .Property(e => e.date_time)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.Request_Date)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.flight_date)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.flight_time)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.EMSContactedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.EMSEnrouteDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.EMSArrivedBNADateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.EMSArrivedBaseDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.BillingApprovedDateTime)
                .HasPrecision(0);

            modelBuilder.Entity<tblFW>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblFWpatient>()
                .Property(e => e.bday)
                .HasPrecision(0);

            modelBuilder.Entity<tblInsurance>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblInsurance>()
                .HasMany(e => e.tblInsContacts)
                .WithOptional(e => e.tblInsurance)
                .WillCascadeOnDelete();

            modelBuilder.Entity<EmailQueue>()
                .Property(e => e.emailaddress)
                .IsFixedLength();

            modelBuilder.Entity<EmailQueue>()
                .Property(e => e.emailbody)
                .IsFixedLength();

            modelBuilder.Entity<EmailQueue>()
                .Property(e => e.emailattachment)
                .IsFixedLength();

            modelBuilder.Entity<EmailQueue>()
                .Property(e => e.emailsent)
                .IsFixedLength();

            modelBuilder.Entity<EmailQueue>()
                .Property(e => e.emailqueued)
                .HasPrecision(0);

            modelBuilder.Entity<tblAccessLog>()
                .Property(e => e.eventdatetime)
                .HasPrecision(0);
        }
    }
}
