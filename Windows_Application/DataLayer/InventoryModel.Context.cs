﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class InventoryContext : DbContext
    {
        public InventoryContext()
            : base("name=InventoryContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AHU> AHUs { get; set; }
        public virtual DbSet<AHUTransaction> AHUTransactions { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<EunKG> EunKGs { get; set; }
        public virtual DbSet<FCU> FCUs { get; set; }
        public virtual DbSet<FCUTransaction> FCUTransactions { get; set; }
        public virtual DbSet<GITransaction> GITransactions { get; set; }
        public virtual DbSet<GoodsReceive> GoodsReceives { get; set; }
        public virtual DbSet<GRTransaction> GRTransactions { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<ModuleAccessCtrl> ModuleAccessCtrls { get; set; }
        public virtual DbSet<ModuleAccessCtrlTransaction> ModuleAccessCtrlTransactions { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
