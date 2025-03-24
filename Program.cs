using Microsoft.EntityFrameworkCore;
using RailwayReservationMVC.Data;
using RailwayReservationMVC.Services; 

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<QuotaService>();  

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(); 
builder.Services.AddHttpContextAccessor(); 

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); 

app.Run();
