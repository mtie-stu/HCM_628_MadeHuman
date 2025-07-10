using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Inbound
{
    public interface IInboundReciptService
    {
        Task<InboundReceipts> CreateAsync(CreateInboundReceiptViewModel vm);
        Task<InboundReceipts?> GetByIdAsync(Guid receiptId);
        Task<List<InboundReceipts>> GetAllAsync();
        Task<bool> CancelInboundReceiptAsync(Guid id);
    }
    public class InboundReciptSvc : IInboundReciptService
    {
        private readonly ApplicationDbContext _context;

        public InboundReciptSvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InboundReceipts> CreateAsync(CreateInboundReceiptViewModel vm)
        {
            var idreceipt = Guid.NewGuid();

            var receipt = new InboundReceipts
            {
                Id = idreceipt,
                CreateAt = DateTime.UtcNow,
                Status = StatusReceipt.Created,
                InboundReceiptItems = new List<InboundReceiptItems>()
            };

            // 1. Lấy danh sách SKU trước
            var skuIds = vm.Items.Select(i => i.ProductSKUId).ToList();
            var skuMap = await _context.ProductSKUs
                .Where(sku => skuIds.Contains(sku.Id))
                .ToDictionaryAsync(sku => sku.Id);

            // 2. Tạo từng receipt item
            foreach (var item in vm.Items)
            {
                if (!skuMap.TryGetValue(item.ProductSKUId, out var sku))
                    throw new Exception($"Không tìm thấy ProductSKUId: {item.ProductSKUId}");

                var receiptItem = new InboundReceiptItems
                {
                    Id = Guid.NewGuid(),
                    InboundReceiptId = idreceipt,
                    Quantity = item.Quantity,
                    ProductSKUId = item.ProductSKUId,
                    ProductSKUs = sku
                };

                receipt.InboundReceiptItems.Add(receiptItem);
            }

            _context.InboundReceipt.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }



        public async Task<InboundReceipts?> GetByIdAsync(Guid receiptId)
        {
            return await _context.InboundReceipt
                .Include(r => r.InboundTasks)
                .Include(r => r.InboundReceiptItems) // cần sửa model để có navigation property ngược
                .FirstOrDefaultAsync(r => r.Id == receiptId);
        }

        public async Task<List<InboundReceipts>> GetAllAsync()
        {
            return await _context.InboundReceipt
                .Include(r => r.InboundTasks)
                .Include(r => r.InboundReceiptItems)
                .ToListAsync();
        }

        public async Task<bool> CancelInboundReceiptAsync(Guid id)
        {
            var receipt = await _context.InboundReceipt.FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                throw new ArgumentException("InboundReceipt không tồn tại.");

            if (receipt.Status == StatusReceipt.Cancel)
                return false; // đã bị hủy rồi

            receipt.Status = StatusReceipt.Cancel;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

