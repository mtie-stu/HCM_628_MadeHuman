using MadeHuman_Server.Data;
using MadeHuman_Server.JwtMiddleware;
using MadeHuman_Server.Model.User_Task;
using MadeHuman_Server.Service;
using MadeHuman_Server.Service.Inbound;
using MadeHuman_Server.Service.Outbound;
using MadeHuman_Server.Service.Shop;
using MadeHuman_Server.Service.UserTask;
using MadeHuman_Server.Service.WareHouse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ⚙️ Logging hỗ trợ debug
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 🗄️ Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.CommandTimeout(180) // ⏰ tăng timeout lên 3 phút
    ));
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection1")));*/

// Thêm cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7112") // Giao diện FE
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Nếu dùng Cookie
    });
});

// 👤 Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔐 JWT Auth
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AuthSettings:Audience"],
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["AuthSettings:Key"]!)),
        ValidateIssuerSigningKey = true
    };
});

// 🧩 Services
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IWarehouseService, WareHouseSvc>();
builder.Services.AddScoped<IWarehouseZoneService, WareHouseZoneSvc>();
builder.Services.AddScoped<IWarehouseLocationService, WareHouseLocationSvc>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShopOrderService, ShopOrderService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationSvc>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<ISkuGeneratorService, SkuGeneratorService>();
builder.Services.AddScoped<ISKUServices, SKUSvc>();
builder.Services.AddScoped<IInboundReciptService, InboundReciptSvc>();
builder.Services.AddScoped<IRefillTaskService, RefillTaskService>();
builder.Services.AddHostedService<ReceiptStatusUpdaterService>();
builder.Services.AddScoped<GoogleSheetService>();
builder.Services.AddScoped<IPartTimeCompanyService, PartTimeCompanySvc>();
builder.Services.AddScoped<IPartTimeService, PartTimeService>();
builder.Services.AddScoped<IPartTimeAssignmentService, PartTimeAssignmentService>();
builder.Services.AddScoped<IUserTaskSvc, UserTaskSvc>();
builder.Services.AddScoped<IInboundTaskSvc, InboundTaskSvc>();
builder.Services.AddHostedService<ResetHourlyKPIsService>();
builder.Services.AddSingleton<GoogleDriveService>();
builder.Services.AddSingleton<GoogleDriveOAuthService>();
builder.Services.AddHostedService<InventoryQuantityUpdateService>();
builder.Services.AddHostedService<OutboundTaskBackgroundService>();
builder.Services.AddScoped<OutboundTaskService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IBasketService, BasketService>();

// (Tùy chọn) Cấu hình upload file lớn nếu cần
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});
// 📦 Controller & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MadeHuman API", Version = "v1" });

    // 🛠️ Tránh lỗi trùng schemaId giữa 2 class cùng tên khác namespace
    opt.CustomSchemaIds(type => type.FullName);

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Nhập token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ✅ Seed dữ liệu gốc trước
    await ApplicationDbContext.SeedPartTimeAsync(db);

    // ✅ Sau đó mới seed Assignment
    //await ApplicationDbContext.SeedPartTimeAssignmentAsync(db);
}


// Configure the HTTP request pipeline.
// 🧪 Hiển thị lỗi chi tiết khi dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 🌐 Swagger cho mọi môi trường
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MadeHuman API v1");
});
// Bật CORS trước UseAuthorization()
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
