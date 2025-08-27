
using MadeHuman_User.JWT;
using MadeHuman_User.ServicesTask.Services;
using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.ServicesTask.Services.OutboundService;
using MadeHuman_User.ServicesTask.Services.ShopService;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

//cấu hình service
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IShopOrderService, ShopOrderService>();
builder.Services.AddScoped<IInboundReceiptService, InboundReceiptService>();
builder.Services.AddScoped<IInboundTaskService, InboundTaskService>();
builder.Services.AddScoped<ICheckinCheckoutService, CheckinCheckoutService>();
builder.Services.AddScoped<IRefillTaskService, RefillTaskService>();
builder.Services.AddScoped<IPickTaskApiService, PickTaskApiService>();
builder.Services.AddScoped<IWarehouseLocationServices, WarehouseLocationServices>();
builder.Services.AddScoped<IBillRenderService, BillRenderService>();
builder.Services.AddScoped<ICheckTaskServices, CheckTaskServices>();
builder.Services.AddScoped<IPackTaskService, PackTaskService>();
builder.Services.AddScoped<IDispatchTaskService, DispatchTaskService>();
builder.Services.AddHttpContextAccessor();


/*https://hcm-628-madehuman-api.onrender.com*/
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://hcm-628-madehuman-api.onrender.com"); // Đảm bảo URL chính xác

    //client.BaseAddress = new Uri("https://localhost:7204"); // Đảm bảo URL chính xác

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<JwtMiddleware>(); // ⬅️ Trước Authentication
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
// pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
