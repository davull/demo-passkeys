using DemoPasskeys;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app
    .UseSwagger()
    .UseSwaggerUI();

Seed.SeedUsers();

app.Run();

public partial class Program;