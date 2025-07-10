
using MadeHuman_User.ServicesTask.Services;
using MadeHuman_User.ServicesTask.Services.ShopService;

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



builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://hcm-628-madehuman-api.onrender.com"); // Đảm bảo URL chính xác
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

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
// pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
