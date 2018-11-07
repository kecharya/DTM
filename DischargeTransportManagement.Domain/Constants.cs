using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargeTransportManagement.Domain
{
    public class Constants
    {
        public enum Pavillions : int
        {
            Unknown=0, 
            VanderbiltHospital =1,
            ChildrensHospital=3,
            PsychiatriacHospital=5,
            BillWilkersonCenter=7,
            VanderbiltheartVasInst=4,
            Adult=8
        }

        public static class RequestStatusMaster
        {
            public const int Confirmed = 1;
            public const int Cancelled = 2;
            public const int Pending = 3;
            public const int Quote = 4;
            public const int Scheduled = 5;
            
        }

        public static class SpecialNeedMaster
        {
            public const int Oxygen = 1;
            public const int IVMeds = 2;
            public const int CardiacMonitor = 3;
            public const int Pacer = 4;
            public const int Trach = 5;
            public const int Ventilator = 6;
        }
    }
}
