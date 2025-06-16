using MadeHuman_Server.Data;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Microsoft.EntityFrameworkCore;
namespace MadeHuman_Server.Service.WareHouse
{

    public interface IWarehouseService
    {
        Task<WareHouseViewModel>CreateAsync(WareHouseViewModel warehouse);
        Task<WareHouseViewModel> UpdateAsync(Guid id, WareHouseViewModel warehouse);
        Task<WareHouseViewModel> GetByIdAsync(Guid id);
        Task<List<WareHouseViewModel>> GetAllAsync();
    }
    public class WareHouseSvc : IWarehouseService
    {
        protected ApplicationDbContext _context;
        public WareHouseSvc(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<WareHouseViewModel> CreateAsync(WareHouseViewModel warehouseVm)
        {
            var warehouse = new Model.WareHouse.WareHouse
            {
                Id = Guid.NewGuid(),
                Name = warehouseVm.Name,
                Location = warehouseVm.Location,
                LastUpdated = DateTime.UtcNow
            };

            _context.WareHouses.Add(warehouse);
            await _context.SaveChangesAsync();

            // Trả lại ViewModel nếu cần
            return new WareHouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                LastUpdated = warehouse.LastUpdated
            };
        }


        public async Task<List<WareHouseViewModel>> GetAllAsync()
        {
            return await _context.WareHouses
                .Select(wh => new WareHouseViewModel
                {
                    Id = wh.Id,
                    Name = wh.Name,
                    Location = wh.Location,
                    LastUpdated = wh.LastUpdated
                })
                .ToListAsync(); // cần using Microsoft.EntityFrameworkCore
        }

        public async Task<WareHouseViewModel> GetByIdAsync(Guid id)
        {
            var warehouse = await _context.WareHouses.FindAsync(id);
            if (warehouse == null) return null;

            return new WareHouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                LastUpdated = warehouse.LastUpdated,
             
            };
        }

        public async Task<WareHouseViewModel> UpdateAsync(Guid id, WareHouseViewModel warehouse)
        {
            var updatewarehouse = await _context.WareHouses.FindAsync(id);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            warehouse.Name = warehouse.Name;
            warehouse.Location = warehouse.Location;
            warehouse.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            return warehouse;
        }
    }
}
