using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Madehuman_User.ViewModel.WareHouse;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MadeHuman_Server.Service.WareHouse
{
    public interface IWarehouseLocationService
    {
        Task<WarehouseLocationViewModel> CreateAsync(WarehouseLocationViewModel warehouse);
        Task<WarehouseLocationViewModel> UpdateAsync(Guid id, WarehouseLocationViewModel warehouse);
        Task<WarehouseLocationViewModel> GetByIdAsync(Guid id);
        Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId);

        Task<List<WarehouseLocationViewModel>> GetAllAsync();
        Task<List<WarehouseLocationViewModel>> GenerateLocationsAsync(
     Guid zoneId,
     char startLetter,
     char endLetter,
     int startNumber,
     int endNumber,
     int startSub,
     int endSub,
     int quantity);
        
    }
    public class WareHouseLocationSvc : IWarehouseLocationService
    {
        protected ApplicationDbContext _context;
        public WareHouseLocationSvc(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<WarehouseLocationViewModel> CreateAsync(WarehouseLocationViewModel warehouse)
        {
            var newId = Guid.NewGuid();

            var createwarehouselocation = new WarehouseLocations
            {
                Id = newId,
                NameLocation = warehouse.NameLocation,
                ZoneId = warehouse.ZoneId,
                StatusWareHouse= StatusWareHouse.Empty
            };

            _context.WarehouseLocations.Add(createwarehouselocation);

            // Tạo inventory tương ứng
            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductSKUId = null,
                StockQuantity = 0,
                QuantityBooked = 0,
                LastUpdated = DateTime.Now,
                WarehouseLocationId = newId // sửa tại đây
            };

            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            return new WarehouseLocationViewModel
            {
                Id = newId, // sửa tại đây để phản ánh ID thật sự đã lưu
                NameLocation = warehouse.NameLocation,
                ZoneId = warehouse.ZoneId,
            };
        }


        public async Task<List<WarehouseLocationViewModel>> GetAllAsync()
        {
            return await _context.WarehouseLocations
                 .Select(wh => new WarehouseLocationViewModel
                 {
                     Id = wh.Id,
                     NameLocation = wh.NameLocation,
                     ZoneId = wh.ZoneId,
                 })
                 .ToListAsync(); // cần using Microsoft.EntityFrameworkCore
        }

        public async Task<WarehouseLocationViewModel> GetByIdAsync(Guid id)
        {
            var warehouse = await _context.WarehouseLocations.FindAsync(id);
            if (warehouse == null) return null;

            return new WarehouseLocationViewModel
            {
                Id = warehouse.Id,
                NameLocation = warehouse.NameLocation,
                ZoneId = warehouse.ZoneId,

            };
        }

        public async Task<WarehouseLocationViewModel> UpdateAsync(Guid id, WarehouseLocationViewModel warehouse)
        {
            var updatewarehouse = await _context.WarehouseLocations.FindAsync(id);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            warehouse.NameLocation = warehouse.NameLocation;
            warehouse.ZoneId = warehouse.ZoneId;

            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<List<WarehouseLocationViewModel>> GenerateLocationsAsync(
            Guid zoneId,
            char startLetter,
            char endLetter,
            int startNumber,
            int endNumber,
            int startSub,
            int endSub,
            int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (startLetter > endLetter)
                throw new ArgumentException("StartLetter phải nhỏ hơn hoặc bằng EndLetter.");

            if (startNumber > endNumber || startSub > endSub)
                throw new ArgumentException("Khoảng Number hoặc Sub không hợp lệ.");

            var zone = await _context.WarehouseZones
                .FirstOrDefaultAsync(z => z.Id == zoneId)
                ?? throw new ArgumentException("ZoneId không tồn tại.");

            string prefix = zone.Name switch
            {
                "Inbound" => "VNIS",
                "Outbound" => "VNOS",
                _ => "VNVS"
            };

            var createdLocations = new List<WarehouseLocationViewModel>();
            int count = 0;

            var existingNames = new HashSet<string>(
                await _context.WarehouseLocations
                    .Where(x => x.ZoneId == zoneId)
                    .Select(x => x.NameLocation)
                    .ToListAsync());

            for (char letter = startLetter; letter <= endLetter && count < quantity; letter++)
            {
                for (int number = startNumber; number <= endNumber && count < quantity; number++)
                {
                    for (int sub = startSub; sub <= endSub && count < quantity; sub++)
                    {
                        string formattedNumber = number.ToString("D3");
                        string name = $"{prefix}-{letter}-{formattedNumber}-{sub}";

                        if (existingNames.Contains(name))
                            continue;

                        // Tạo entity location
                        var location = new WarehouseLocations
                        {
                            Id = Guid.NewGuid(),
                            NameLocation = name,
                            ZoneId = zoneId,
                            StatusWareHouse = StatusWareHouse.Empty
                        };

                        _context.WarehouseLocations.Add(location);

                        // Tạo inventory tương ứng
                        var inventory = new Inventory
                        {
                            Id = Guid.NewGuid(),
                            ProductSKUId = null,
                            StockQuantity = 0,
                            QuantityBooked = 0,
                            LastUpdated = DateTime.UtcNow,
                            WarehouseLocationId = location.Id
                        };

                        _context.Inventory.Add(inventory);

                        // ✅ Tạo LowStockAlert mặc định
                        var lowStockAlert = new LowStockAlerts
                        {
                            Id = Guid.NewGuid(),
                            WarehouseLocationId = location.Id,
                            CurrentQuantity = 0,
                            StatusLowStockAlerts = StatusLowStockAlerts.Empty
                        };

                        _context.LowStockAlerts.Add(lowStockAlert);

                        // Add vào kết quả trả về
                        createdLocations.Add(new WarehouseLocationViewModel
                        {
                            Id = location.Id,
                            NameLocation = location.NameLocation,
                            ZoneId = location.ZoneId
                        });

                        count++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return createdLocations;
        }



        public async Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId)
        {
            return await _context.WarehouseLocations
                .Where(w => w.Id == warehouseLocationId)
                .Select(w => new WarehouseLocationInfoViewModel
                {
                    WarehouseLocationId = w.Id,
                    NameLocation = w.NameLocation
                })
                .FirstOrDefaultAsync();
        }

    }
}
