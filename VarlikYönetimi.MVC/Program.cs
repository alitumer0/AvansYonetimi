using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Services;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Services.Services;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Mapping.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Interfaces;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapProfile));


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
}, ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IApprovalSettingsRepository, ApprovalSettingsRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAdvanceRequestRepository, AdvanceRequestRepository>();
builder.Services.AddScoped<IApprovalProcessRepository, ApprovalProcessRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

var emailConfig = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddScoped<IEmailService>(provider => new EmailService(
    emailConfig["SmtpServer"],
    int.Parse(emailConfig["SmtpPort"]),
    emailConfig["SmtpUsername"],
    emailConfig["SmtpPassword"],
    emailConfig["FromEmail"]
));

builder.Services.AddScoped<IApprovalSettingsService, ApprovalSettingsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAdvanceRequestService, AdvanceRequestService>();
builder.Services.AddScoped<IApprovalProcessService, ApprovalProcessService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAdvanceLimitService, AdvanceLimitService>();
builder.Services.AddScoped<ILegalActionService, LegalActionService>();
builder.Services.AddScoped<IDepartmentApprovalLimitService, DepartmentApprovalLimitService>();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    await UserSeed.SeedAdminUser(userManager, roleManager);
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILegalActionService, LegalActionService>();
builder.Services.AddHostedService<AdvanceRequestTimeoutService>();

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IAdvanceRequestService, AdvanceRequestService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDepartmentApprovalLimitService, DepartmentApprovalLimitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
