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
    
    public partial class GITransaction
    {
        public long ID { get; set; }
        public string Text { get; set; }
        public decimal Quantity { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public long GRID { get; set; }
        public string TransferType { get; set; }
        public string ProductionNo { get; set; }
        public string LocationTo { get; set; }
        public string LocationFrom { get; set; }
    
        public virtual GoodsReceive GoodsReceive { get; set; }
    }
}