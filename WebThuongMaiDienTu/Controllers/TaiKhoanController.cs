using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
            shopDienThoaiEntities db = new shopDienThoaiEntities();
            List<TaiKhoan> taiKhoans = db.TaiKhoan.ToList();
            return View(taiKhoans);
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

            bool isCapthcaValid = ValidateCaptcha(Request["g-recaptcha-response"]);
            if (ModelState.IsValid)
            {
                if (isCapthcaValid)
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
                                    //ModelState.AddModelError("", "Tên tài khoản hoặc email đã tồn tại.");
                                    //return View(model);
                                    string script = "alert('Email tồn tại');history.back();";
                                    return Content("<script type='text/javascript'>" + script + "</script>");
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
                                    ngayTao = DateTime.Now,
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
                                //ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
                                //return View(model);
                                string script = "alert('Lỗi Đăng ký vui lòng thử lại sao');history.back();";
                                return Content("<script type='text/javascript'>" + script + "</script>");
                            }
                        }
                    }

                    return RedirectToAction("DangNhap", "TaiKhoan");
                }
                else
                {
                    

                    //Should load sitekey again
                    return View();
                }
            }
            return View();


            
        }


        [AllowAnonymous]
        public bool ValidateCaptcha(string response)
        {
            //  Setting _Setting = repositorySetting.GetSetting;

            //secret that was generated in key value pair  
            string secret = ConfigurationManager.AppSettings["GoogleSecretkey"]; //WebConfigurationManager.AppSettings["recaptchaPrivateKey"];

            var client = new WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            return Convert.ToBoolean(captchaResponse.Success);

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