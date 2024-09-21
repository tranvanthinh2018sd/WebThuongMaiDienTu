using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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

                        // Giỏ Hàng
                        var gioHang = new GioHang
                        {
                            maKhachHang = khachHang.maKhachHang,
                        };
                        db.GioHang.Add(gioHang);
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
        public ActionResult DangNhap(DangNhapViewModel model, string returnUrl)
        {
            using (var db = new shopDienThoaiEntities())
            {
                var taiKhoan = db.TaiKhoan.SingleOrDefault(t => t.tenTaiKhoan == model.tenTaiKhoan);

                if (taiKhoan == null || taiKhoan.matKhau != HashPassword(model.matKhau))
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // Đăng nhập thành công
                FormsAuthentication.SetAuthCookie(taiKhoan.tenTaiKhoan, false);
                // Thêm thông tin tài khoản vào Session
                Session["TaiKhoan"] = taiKhoan; // Lưu đối tượng `TaiKhoan` vào session

                // Chuyển hướng về trang yêu cầu hoặc trang mặc định
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa toàn bộ session
            return RedirectToAction("DangNhap", "TaiKhoan");
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