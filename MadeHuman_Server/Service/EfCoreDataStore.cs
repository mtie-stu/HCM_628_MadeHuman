using Google.Apis.Util.Store;
using MadeHuman_Server.Data;
using MadeHuman_Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public class EfCoreDataStore : IDataStore
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EfCoreDataStore(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private static string MakeKey<T>(string key) => $"{typeof(T).FullName}-{key}";

    public async Task StoreAsync<T>(string key, T value)
    {
        var k = MakeKey<T>(key);
        var json = JsonConvert.SerializeObject(value);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existing = await db.OAuthData.FirstOrDefaultAsync(x => x.Key == k);
        if (existing == null)
        {
            db.OAuthData.Add(new OAuthDataItem
            {
                Key = k,
                Value = json,
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.Value = json;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var k = MakeKey<T>(key);
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var row = await db.OAuthData.AsNoTracking().FirstOrDefaultAsync(x => x.Key == k);
        if (row == null) return default;
        return JsonConvert.DeserializeObject<T>(row.Value);
    }

    public async Task DeleteAsync<T>(string key)
    {
        var k = MakeKey<T>(key);
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var row = await db.OAuthData.FirstOrDefaultAsync(x => x.Key == k);
        if (row != null)
        {
            db.OAuthData.Remove(row);
            await db.SaveChangesAsync();
        }
    }

    public async Task ClearAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.OAuthData.RemoveRange(db.OAuthData);
        await db.SaveChangesAsync();
    }
}
