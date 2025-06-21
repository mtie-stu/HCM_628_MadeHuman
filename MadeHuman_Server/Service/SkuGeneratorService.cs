using MadeHuman_Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MadeHuman_Server.Service
{
    public interface ISkuGeneratorService
    {
        Task<string> GenerateUniqueSkuAsync();
    }
    public class SkuGeneratorService : ISkuGeneratorService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();

        public SkuGeneratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueSkuAsync()
        {
            string sku;
            int attempt = 0;
            const int maxAttempts = 10;

            do
            {
                sku = GenerateRandomSku(20);
                attempt++;

                // kiểm tra trong DB có tồn tại chưa
                bool exists = await _context.ProductSKUs.AnyAsync(s => s.SKU == sku);
                if (!exists) return sku;

            } while (attempt < maxAttempts);

            throw new Exception("Không thể tạo SKU duy nhất sau nhiều lần thử.");
        }

        private string GenerateRandomSku(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[_random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }

}
