using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        public ActionResult Index()
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
                Session["returnUrl"] = Url.Action("Index", "KhachHang");
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            // Lọc sản phẩm có kichHoat == true và sắp xếp theo maSanPham giảm dần
            List<KhachHang> khachHangs = db.KhachHang
                .Where(row => row.kichHoat == true) // Điều kiện lọc
                .OrderByDescending(row => row.maKhachHang) // Sắp xếp giảm dần theo maSanPham
                .ToList();

            return View(khachHangs);
        }
        //Edit
        public ActionResult Edit(int maKhachHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
                Session["returnUrl"] = Url.Action("Edit", "KhachHang");
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            KhachHang khachHang = db.KhachHang.Where(row => row.maKhachHang == maKhachHang).FirstOrDefault();
            return View(khachHang);
        }
        [HttpPost]
        public ActionResult Edit(KhachHang khachhang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            KhachHang khachHang = db.KhachHang.Where(row => row.maKhachHang == khachhang.maKhachHang).FirstOrDefault();
            khachHang.maKhachHang = khachhang.maKhachHang;
            khachHang.hoTen = khachhang.hoTen;
            khachHang.diaChiGiaoHang = khachhang.diaChiGiaoHang;
            khachHang.soDienThoai = khachhang.soDienThoai;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}