using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(WebThuongMaiDienTu.Startup))]
namespace WebThuongMaiDienTu
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/TaiKhoan/DangNhap"), // Đường dẫn đến trang đăng nhập
                LogoutPath = new PathString("/TaiKhoan/DangXuat"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        // Lưu URL yêu cầu trước khi chuyển hướng đến trang đăng nhập
                        if (!ctx.Request.Path.StartsWithSegments(ctx.Options.LoginPath))
                        {
                            var redirectUrl = ctx.Request.PathBase + ctx.Request.Path + ctx.Request.QueryString;
                            ctx.Response.Redirect(ctx.Options.LoginPath + "?returnUrl=" + HttpUtility.UrlEncode(redirectUrl));
                        }
                    }
                }
            });

            // Thêm các middleware khác nếu cần
        }
    }
}