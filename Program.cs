using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Infrastructure.Common;
using TopSpeed.Infrastructure.Repositories;
using TopSpeed.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Identity.UI.Services;
using TopSpeed.Application.Services;
using TopSpeed.Application.Services.Interface;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container & database register
builder.Services.AddDbContext<ApplicationDBContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

  
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.LoginPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromSeconds(20);
});

//register for IGenericRepository 
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//register for IUnitOfWork 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//register for IBrandRepository
builder.Services.AddScoped<IBrandRepository, BrandRepository>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<IUserNameService, UserNameService>();

builder.Services.AddHttpContextAccessor();

 
#region Configuration for seeding Data to Database
static async void UpdateDatabaseAsync(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services= scope.ServiceProvider;    
        try
        {
            var context = services.GetRequiredService<ApplicationDBContext>();

            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate(); 
            }

            await SeedData.SeedDataAsync(context);
        }
        catch (Exception ex)
        {
            var  logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
           // var logger = services.GetRequiredService<ILogger>();
            logger.LogError(ex ,"An error occurred while migrating or seeding the database.");
        }
    }
}
#endregion

builder.Host.UseSerilog((context, config)=>
{
    config.WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day);

    if (context.HostingEnvironment.IsProduction() == false)
    {
        config.WriteTo.Console();
    }
});  

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();    

var app = builder.Build();

var serviceProvider = app.Services;
await SeedData.SeedRole(serviceProvider);   

UpdateDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.UseSession();   

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
        
