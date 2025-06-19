using MadeHuman_Server.Data;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.WareHouse
{
    public interface IWarehouseZoneService
    {
        Task<WareHouseZoneViewModel> CreateAsync(WareHouseZoneViewModel warehouse);
        Task<WareHouseZoneViewModel> UpdateAsync(Guid id, WareHouseZoneViewModel warehouse);
        Task<WareHouseZoneViewModel> GetByIdAsync(Guid id);
        Task<List<WareHouseZoneViewModel>> GetAllAsync();


    }
    public class WareHouseZoneSvc : IWarehouseZoneService
    {
        protected ApplicationDbContext _context;
        public WareHouseZoneSvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WareHouseZoneViewModel> CreateAsync(WareHouseZoneViewModel warehousezone)
        {
            var createwarehousezone = new Model.WareHouse.WarehouseZones
            {
                Id = Guid.NewGuid(),
                Name = warehousezone.Name,
                WarehouseId = warehousezone.WarehouseId
            };

            _context.WarehouseZones.Add(createwarehousezone);
            await _context.SaveChangesAsync();

            // Trả lại ViewModel nếu cần
            return new WareHouseZoneViewModel
            {
                Id = warehousezone.Id,
                Name = warehousezone.Name,
                WarehouseId = warehousezone.WarehouseId,
                
            };
        }

        public async Task<List<WareHouseZoneViewModel>> GetAllAsync()
        {
            return await _context.WarehouseZones
                 .Select(wh => new WareHouseZoneViewModel
                 {
                     Id = wh.Id,
                     Name = wh.Name,
                     WarehouseId = wh.WarehouseId,
                 })
                 .ToListAsync(); // cần using Microsoft.EntityFrameworkCore
        }

        public async Task<WareHouseZoneViewModel> GetByIdAsync(Guid id)
        {
            var warehouse = await _context.WarehouseZones.FindAsync(id);
            if (warehouse == null) return null;

            return new WareHouseZoneViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                WarehouseId = warehouse.WarehouseId,

            };
        }

        public async Task<WareHouseZoneViewModel> UpdateAsync(Guid id, WareHouseZoneViewModel warehouse)
        {
            var updatewarehouse = await _context.WarehouseZones.FindAsync(id);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            warehouse.Name = warehouse.Name;
            warehouse.WarehouseId = warehouse.WarehouseId;

            await _context.SaveChangesAsync();
            return warehouse;
        }
    }
}
