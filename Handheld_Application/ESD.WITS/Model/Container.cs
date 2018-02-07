using System;

namespace ESD.WITS.Model
{
    public class Container
    {
        public int ID { get; set; }

        public string PartNumber { get; set; }

        public string Description { get; set; }

        public string MachineUsage { get; set; }

        public decimal Cost { get; set; }

        public int CurrencyID { get; set; }

        public int MinQty { get; set; }

        public int MaxQty { get; set; }

        public int SafetyStockLevel { get; set; }

        public int SupplierID { get; set; }

        public string Enabled { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
