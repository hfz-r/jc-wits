//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class FCUTransaction
    {
        public long ID { get; set; }
        public decimal Quantity { get; set; }
        public long FCUID { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public long CountryID { get; set; }
    
        public virtual FCU FCU { get; set; }
        public virtual Country Country1 { get; set; }
    }
}