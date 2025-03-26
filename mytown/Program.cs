using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using mytown.DataAccess;
using mytown.Services;


void TestMySQLConnection()
{
      string connStr = "Server=mytown-mysql-db.mysql.database.azure.com;Port=3306;Database=mytown_db;User Id=dbadmin;Password=admin4321$;SslMode=REQUIRED;TlsVersion=TLSv1.2;";
  //  string connStr = "Server=mytown-mysql-db.mysql.database.azure.com;Port=3306;Database=mytown_db;User Id=dbadmin@mytown-mysql-db;Password=admin4321$;SslMode=VerifyCA;TlsVersion=TLSv1.2;SslCa=C:\\certs\\DigiCertGlobalRootCA.crt.pem;";

    using (var conn = new MySqlConnection(connStr))
    {
        try
        {
            conn.Open();
            Console.WriteLine(" Connection successful now!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(" Connection failed: " + ex.Message);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); // Ensure services are configured


TestMySQLConnection();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IShopperRepository, ShopperRepository>();

// Enable CORS to allow requests from http://localhost:3003
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Allow only this origin
              .AllowAnyMethod()                      // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();                     // Allow any headers (e.g., Content-Type)
    });
});

builder.Services.AddAuthentication();
try { 
var app = builder.Build();

// **Apply CORS Middleware Before Routing**
app.UseCors("AllowFrontend");  // Enable CORS with the "AllowFrontend" policy

// Configure the HTTP request pipeline.
startup.Configure(app, builder.Environment);

// Enable Routing and Map Controllers (If you have API controllers)
app.UseRouting();
    app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex.Message}");
throw;
}