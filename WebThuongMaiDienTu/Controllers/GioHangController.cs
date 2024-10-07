using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        public ActionResult Index()
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<GioHang> gioHangs = db.GioHang.OrderByDescending(row => row.maGioHang).ToList();
            return View(gioHangs);
        }
        
        public ActionResult IndexUser()
        {
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<GioHang> gioHangs = db.GioHang.OrderByDescending(row => row.maGioHang).Where(row => row.maKhachHang == taiKhoan.maKhachHang).ToList();
            return View(gioHangs);
        }

        //Edit
        public ActionResult Edit(int maGioHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ViewBag.KhachHang = db.KhachHang.ToList();
            GioHang gioHang = db.GioHang.Where(row => row.maGioHang == maGioHang).FirstOrDefault();
            return View(gioHang);
        }
        [HttpPost]
        public ActionResult Edit(GioHang gioHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
                GioHang giohang = db.GioHang.Where(row => row.maGioHang == gioHang.maGioHang).FirstOrDefault();
                giohang.maGioHang = gioHang.maGioHang;
                giohang.maKhachHang = giohang.maKhachHang;
                giohang.ngayTao = giohang.ngayTao;
                db.SaveChanges();
                return RedirectToAction("Index");         

        }
        
    }
}