using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TeachingAI1.Data;
using TeachingAI1.Services;
using TeachingAI1.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using TeachingAI1.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources"); // 资源文件存放根目录
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) // 视图本地化（按后缀匹配资源文件）
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),    // 英文
        new CultureInfo("zh-CN"), // 简体中文
        new CultureInfo("zh")     // 中文（备用）
    };

    // 设置默认文化、支持的文化
    options.DefaultRequestCulture = new RequestCulture("zh-CN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddControllers();
builder.Services.Configure<DashScopeQuizOptions>(
    builder.Configuration.GetSection(DashScopeQuizOptions.SectionName));
builder.Services.AddHttpClient(); // 用于注入 IHttpClientFactory

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAiQuizService, AiQuizService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add session services
builder.Services.AddDistributedMemoryCache();


//Enable session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(QuizOptions =>
{
    QuizOptions.IdleTimeout = TimeSpan.FromMinutes(30); //Setting session timeout
    QuizOptions.Cookie.HttpOnly = true; //Make session cookies Http-only for security
    QuizOptions.Cookie.IsEssential = true; //Required for GDPR compliance
});

//Need to add my 
builder.Services.AddDbContext<ApplicationDbContext>(QuizOptions =>
    QuizOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()); // Log SQL queries and connection attempts

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(QuizOptions =>
{
    QuizOptions.LoginPath = "/Account/Login"; //Path to login page
    // QuizOptions.LoginPath = "/Account/Logout"; //Path to logout page
});

// 注册 QwenService
builder.Services.AddHttpClient<QwenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();
app.UseStaticFiles(); //Need to make sure the static files are served
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/Home/Index"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var controller = typeof(CourseController);

var assembly = typeof(Program).Assembly;

app.Run();
