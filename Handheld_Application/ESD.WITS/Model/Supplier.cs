using System;

namespace ESD.WITS.Model
{
    public class Supplier
    {
        public int ID { get; set; }

        public string SupplierName { get; set; }

        public string ContactPerson { get; set; }

        public string ContactTelNo { get; set; }

        public string ContactFaxNo { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Enabled { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
