using EduCourse.SeedDataMigration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;
using TIE_Decor.MiddleWare;
using TIE_Decor.Models;
using TIE_Decor.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<ITrackingService, TrackingService>();
        // Cấu hình Identity
        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 0;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(24); // Thời gian hết hạn của Session
            options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập qua HTTP
            options.Cookie.IsEssential = true; // Đảm bảo cookie luôn được gửi đi
        });
        // Cấu hình Cookie
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
        });

        // Cấu hình DbContext
        builder.Services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer(builder.Configuration.GetConnectionString("Connection"));
        });
        builder.Services.AddControllersWithViews();

        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        var app = builder.Build();
        StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }
        app.UseDeveloperExceptionPage();
        app.UseMiddleware<PageViewTrackingMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
             name: "area",
             pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        // Khởi tạo roles khi ứng dụng bắt đầu
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                await SeedRoles.Initialize(services);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating roles");
            }
        }

        await app.RunAsync();
    }
}
