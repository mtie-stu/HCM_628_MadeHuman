using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Madehuman_User.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Shop
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(CreateCategoryViewModel vm);
        Task<bool> UpdateAsync(Guid id, CreateCategoryViewModel vm);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateAsync(CreateCategoryViewModel vm)
        {
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = vm.Name,
                Description = vm.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Guid id, CreateCategoryViewModel vm)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            category.Name = vm.Name;
            category.Description = vm.Description;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(e => e.CategoryId == id))
                    return false;
                throw;
            }
        }
    }
}
