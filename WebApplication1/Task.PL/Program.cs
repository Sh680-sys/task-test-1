using System;
using System.Text;
using AutoMapper;
using BLL.Interfaces;
using BLL.MappingProfiles;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Task.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// DbContext
// -----------------------------
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

// -----------------------------
// Repositories and UoW
// -----------------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// -----------------------------
// Services (BLL)
// -----------------------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Email sender & JWT services
builder.Services.AddScoped<IEmailSender, EmailSender>(); // خدمة إرسال الإيميل (SMTP)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<JwtService>(); // خدمة توليد التوكنات
// DI

// -----------------------------
// AutoMapper
// -----------------------------
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<DomainProfile>());






// -----------------------------
// JWT Authentication setup
// -----------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
if (string.IsNullOrWhiteSpace(jwtKey))
{
    // وإذا ما محطّين المفتاح يطلع خطأ واضح بدل يصير silent fail
    throw new Exception("JWT Key is not configured. Please set Jwt:Key in appsettings.json or environment.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // في التطوير ممكن نخلي RequireHttpsMetadata=false، بالإنتاج خليها true
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30) // توازن بسيط بخصوص فرق التوقيت
    };
});

// -----------------------------
// Controllers + Swagger
// -----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// CORS
// -----------------------------
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// -----------------------------
// Apply Migrations automatically on startup
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// -----------------------------
// Middleware pipeline
// -----------------------------
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // مهم: لازم قبل Authorization و قبل MapControllers
app.UseAuthorization();

app.MapControllers();

app.Run();