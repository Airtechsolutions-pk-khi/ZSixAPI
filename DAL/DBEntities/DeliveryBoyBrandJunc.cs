//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL.DBEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryBoyBrandJunc
    {
        public int DBAJuncID { get; set; }
        public Nullable<int> DeliveryBoyID { get; set; }
        public Nullable<int> BrandID { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual DeliveryBoy DeliveryBoy { get; set; }
    }
}