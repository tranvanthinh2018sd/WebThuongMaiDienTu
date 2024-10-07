using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
        public ActionResult Index()
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
                Session["returnUrl"] = Url.Action("Index", "DonHang");
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<DonHang> donHangs = db.DonHang.OrderByDescending(row => row.maDonHang).ToList();
            return View(donHangs);
        }

            // GET: DonHang/Checkout
            public ActionResult Checkout()
            {
                var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
                if (taiKhoan == null || taiKhoan.taiKhoanAdmin != false)
                {

                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                using (var db = new shopDienThoaiEntities())
                {
                    // Lấy giỏ hàng của khách hàng
                    var gioHang = db.GioHang.FirstOrDefault(g => g.maKhachHang == taiKhoan.maKhachHang);
                    if (gioHang == null)
                    {
                        ModelState.AddModelError("", "Giỏ hàng trống.");
                        return View();
                    }

                    // Tạo đơn hàng mới
                    var donHang = new DonHang
                    {
                        maKhachHang = taiKhoan.maKhachHang,
                        ngayDatHang = DateTime.Now, // Sử dụng thuộc tính ngày đặt hàng
                                                    // Tính toán tổng giá trị của đơn hàng từ giỏ hàng
                        tongGiaTri = db.ChiTietGioHang
                            .Where(ct => ct.maGioHang == gioHang.maGioHang)
                            .Sum(ct => ct.soLuong * ct.donGia) // Tính tổng giá trị
                    };

                    db.DonHang.Add(donHang);
                    db.SaveChanges(); // Lưu đơn hàng để lấy mã đơn hàng

                    // Lấy mã đơn hàng vừa tạo
                    int maDonHang = donHang.maDonHang;

                    // Lấy chi tiết giỏ hàng
                    var chiTietGioHangs = db.ChiTietGioHang.Where(ct => ct.maGioHang == gioHang.maGioHang).ToList();

                    // Thêm từng chi tiết vào đơn hàng
                    foreach (var chiTiet in chiTietGioHangs)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            maDonHang = maDonHang,
                            maSanPham = chiTiet.maSanPham,
                            soLuong = chiTiet.soLuong,
                            donGia = chiTiet.donGia
                        };
                        db.ChiTietDonHang.Add(chiTietDonHang);
                    }

                    db.SaveChanges(); // Lưu chi tiết đơn hàng

                    // Xóa giỏ hàng (tuỳ chọn)
                    db.ChiTietGioHang.RemoveRange(chiTietGioHangs);
                    db.SaveChanges();

                    // Chuyển hướng đến trang xác nhận đơn hàng hoặc trang khác
                    return RedirectToAction("IndexKhachHang", "ChiTietDonHang");
                }
            }
        



        //Edit
        public ActionResult Edit(int maDonHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {// Lưu URL hiện tại (IndexKhachHang) vào Session trước khi chuyển hướng đến trang đăng nhập
                Session["returnUrl"] = Url.Action("Edit", "DonHang");
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            ViewBag.KhachHang = db.KhachHang.ToList();
            DonHang donHang = db.DonHang.Where(row => row.maDonHang == maDonHang).FirstOrDefault();
            return View(donHang);
        }
        [HttpPost]
        public ActionResult Edit(DonHang donHang)
        {
            // Kiểm tra quyền admin
            var taiKhoan = (TaiKhoan)Session["TaiKhoan"];
            if (taiKhoan == null || taiKhoan.taiKhoanAdmin != true)
            {
                // Nếu không có quyền admin, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            DonHang donhang = db.DonHang.Where(row => row.maDonHang == donHang.maDonHang).FirstOrDefault();
            donhang.maDonHang = donHang.maDonHang;
            donhang.maKhachHang = donHang.maKhachHang;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}