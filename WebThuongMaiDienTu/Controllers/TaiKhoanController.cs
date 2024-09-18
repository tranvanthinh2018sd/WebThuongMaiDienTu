using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebThuongMaiDienTu.Models;

namespace WebThuongMaiDienTu.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        public ActionResult Index()
        {
            return View();
        }
        // GET: TaiKhoan/DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: TaiKhoan/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public ActionResult DangKy(DangKyViewModel model)
        {           

            using (var db = new shopDienThoaiEntities())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra xem tài khoản hoặc email đã tồn tại chưa
                        if (db.TaiKhoan.Any(t => t.tenTaiKhoan == model.TenTaiKhoan || t.email == model.Email))
                        {
                            ModelState.AddModelError("", "Tên tài khoản hoặc email đã tồn tại.");
                            return View(model);
                        }

                        // Lưu thông tin khách hàng
                        var khachHang = new KhachHang
                        {
                            hoTen = model.HoTen,
                            diaChiGiaoHang = model.DiaChiGiaoHang,
                            soDienThoai = model.SoDienThoai,
                            kichHoat = true
                        };
                        db.KhachHang.Add(khachHang);
                        db.SaveChanges();

                        // Lưu thông tin tài khoản
                        var taiKhoan = new TaiKhoan
                        {
                            tenTaiKhoan = model.TenTaiKhoan,
                            matKhau = HashPassword(model.MatKhau),
                            email = model.Email,
                            maKhachHang = khachHang.maKhachHang,
                            kichHoat = true,
                            CreatedDate = DateTime.Now
                        };
                        db.TaiKhoan.Add(taiKhoan);
                        db.SaveChanges();

                        // Commit transaction sau khi thành công
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback nếu có lỗi
                        transaction.Rollback();
                        ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
                        return View(model);
                    }
                }
            }

            return RedirectToAction("Index");
        }


        // GET: TaiKhoan/DangNhap
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: TaiKhoan/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(DangNhapViewModel model)
        {
            using (var db = new shopDienThoaiEntities())
            {
                // Tìm tài khoản trong cơ sở dữ liệu theo tên đăng nhập
                var taiKhoan = db.TaiKhoan.SingleOrDefault(t => t.tenTaiKhoan == model.tenTaiKhoan);

                if (taiKhoan == null || taiKhoan.matKhau != HashPassword(model.matKhau))
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // Nếu đăng nhập thành công, bạn có thể thực hiện các hành động như lưu thông tin người dùng vào session
                Session["TaiKhoan"] = taiKhoan;

                return RedirectToAction("Index"); // Hoặc trang nào đó bạn muốn chuyển hướng sau khi đăng nhập thành công
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}