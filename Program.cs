using Microsoft.EntityFrameworkCore;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Services; // ✅ Import QuotaService

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configure Database (SQL Server) using appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Register Services
builder.Services.AddScoped<QuotaService>();  // ✅ Register QuotaService

// 🔹 Add MVC and Session Support
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // Required for session management
builder.Services.AddSession(); // Enable session support
builder.Services.AddHttpContextAccessor(); // Needed to access session data in controllers

var app = builder.Build();

// 🔹 Enable static files (CSS, JavaScript, images)
app.UseStaticFiles();

app.UseRouting();

// 🔹 Enable session middleware
app.UseSession();

app.UseAuthorization();

// 🔹 Define default route (Login page as the first screen)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); 

app.Run();
