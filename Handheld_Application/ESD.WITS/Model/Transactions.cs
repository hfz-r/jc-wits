using System;

namespace ESD.WITS.Model
{
    public class Transactions
    {
        public int ID { get; set; }

        public int ContainerID { get; set; }

        public int Quantity { get; set; }

        public string IsSync { get; set; }

        public string IsOffline { get; set; }

        public string TxnName { get; set; }

        public string Enabled { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
