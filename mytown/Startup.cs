using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mytown.DataAccess;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using Pomelo.EntityFrameworkCore.MySql;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("mysqlConnection");
        Console.WriteLine($"EF Core Connection String: {connectionString}");

        // Configure MySQL connection
        services.AddDbContext<AppDbContext>(options =>
             options.UseMySql(Configuration.GetConnectionString("mysqlConnection"),
                         ServerVersion.Parse("8.0.33-mysql"))); // Change to your actual MySQL version



        // Add repositories or other services
        services.AddScoped<UserRepository>();
        services.AddScoped<IShopperRepository, ShopperRepository>();      
        services.AddScoped<IEmailService, EmailService>();
     

        // Add controllers
        services.AddControllers();

        // Swagger support
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        var allowedOrigins = new List<string>
    {
        "http://localhost:3000", // Local frontend
        "https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net" 
        // Production frontend
    };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(allowedOrigins.ToArray())
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        services.AddControllers();

        // Enable CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:3000")  // Allow the frontend origin (change to your frontend's URL)
                      .AllowAnyMethod()                      // Allow all HTTP methods (GET, POST, etc.)
                      .AllowAnyHeader();                     // Allow all headers (e.g., Content-Type)
            });
        });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseStaticFiles();


        // Enable Swagger middleware
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = "swagger";  // Serve Swagger UI at the app's root
        });

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        //// Apply any pending migrations to the database on startup
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();  // Apply migrations automatically
        }

        // **CORS Middleware should be applied before Routing**
        app.UseRouting();

        // Apply CORS policy
        app.UseCors("AllowFrontend");

        // Map controllers to API endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }



}