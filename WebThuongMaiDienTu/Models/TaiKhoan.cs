//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebThuongMaiDienTu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaiKhoan
    {
        public int maTaiKhoan { get; set; }
        public string tenTaiKhoan { get; set; }
        public bool kichHoat { get; set; }
        public string matKhau { get; set; }
        public int maKhachHang { get; set; }
        public string email { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
    
        public virtual KhachHang KhachHang { get; set; }
    }
}
