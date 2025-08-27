using BL.Contracts;
using BL.Maping;
using BL.Services;
using DAL.Context;

using Exams.Contracts;
using Exams.Models;
using Exams.Repositorys;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Exams
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ إعداد الخدمات
            ConfigureServices(builder);

            var app = builder.Build();
            // ✅ تشغيل SeedData هنا
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ExamsContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await ContextConfig.SeedDataAsync(context, userManager, roleManager);
            }

            ConfigureApp(app);
            await app.RunAsync();
        }
     
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IEaxme, ExamSecrvices>();
            builder.Services.AddScoped<IQuestion, QuestionServices>();
            builder.Services.AddScoped<IChoice, ChoiceServices>();
            builder.Services.AddScoped<BL.Contracts.IResult, ResultServices>();
            builder.Services.AddScoped<IUserServices, UserServices>();
           

            // ✅ ربط `DbContext` بقاعدة البيانات
            builder.Services.AddDbContext<ExamsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ✅ إعداد خدمات الهوية Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ExamsContext>()
                .AddDefaultTokenProviders();
            // ✅ ضبط إعدادات المصادقة
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;    // 🚫 إلغاء شرط وجود رقم
                options.Password.RequiredLength = 6;     // 🔢 الحد الأدنى 6 أحرف فقط
                options.Password.RequireNonAlphanumeric = false; // 🚫 إلغاء شرط الرموز
                options.Password.RequireUppercase = false; // 🚫 إلغاء شرط الحروف الكبيرة
                options.Password.RequireLowercase = false; // 🚫 إلغاء شرط الحروف الصغيرة
            });


            // ✅ إعداد Serilog لتسجيل الأحداث
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
                    tableName: "Log",
                    autoCreateSqlTable: true)
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        private static void ConfigureApp(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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
                    name: "admin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "LandingPages",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=List}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Home",
                    pattern: "{controller=Home}/{action=Exam}/{id?}");

                endpoints.MapControllerRoute(
                    name: "sameh",
                    pattern: "sameh/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}






