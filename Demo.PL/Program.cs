using Demo.DAL.Data;
using Demo.DAL.Models;
using Demo.PL.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            #region Configure Services

            webApplicationBuilder.Services.AddControllersWithViews();

            //webApplicationBuilder.Services.AddScoped<AppDbContext>();

            webApplicationBuilder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //webApplicationBuilder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            //webApplicationBuilder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            webApplicationBuilder.Services.AddApplicationServices();//extinsion Method
            webApplicationBuilder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;//@ # #
                options.Password.RequireDigit = true;//1223
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

            })
                .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            webApplicationBuilder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LogoutPath = "Account/Login";
                    options.AccessDeniedPath = "Home/Error";


                });

            //  ApplicationServicesExtensions.AddApplicationServices(services); //staticMethod 
            #endregion

            var app = webApplicationBuilder.Build();

            #region Cofigure Kestrel MiddleWares

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            }); 
            #endregion

            app.Run();
        }


    }
}
