using MadeHuman_Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Admin.Controllers
{
    public class AdminController : Controller
    {
        private static readonly List<AccountViewModel> _sampleAccounts = new()
        {
            new AccountViewModel { Name = "ABC", Email = "ACE@gmail.com", Role = "Picker", State = "Activate" },
            new AccountViewModel { Name = "AVE", Email = "AVE@gmail.com", Role = "Checker", State = "Lock" },
            new AccountViewModel { Name = "John", Email = "john@example.com", Role = "Dispatch", State = "Activate" },
            new AccountViewModel { Name = "Mary", Email = "mary@example.com", Role = "Packer", State = "Lock" }
        };
        // GET: hiển thị và lọc
        [HttpGet]
        public IActionResult Permission(string? searchTerm, string? role)
        {
            var filtered = _sampleAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filtered = filtered.Where(a =>
                    a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                filtered = filtered.Where(a => a.Role == role);
            }

            var model = new AccountFilterViewModel
            {
                SearchTerm = searchTerm,
                RoleFilter = role,
                Accounts = filtered.ToList()
            };

            return View(model);
        }


        // POST: xử lý lọc + toggle trạng thái
        [HttpPost]
        public IActionResult Permission(AccountFilterViewModel model, string? actionType, string? targetEmail)
        {
            // Toggle trạng thái nếu có yêu cầu
            if (actionType == "toggle" && !string.IsNullOrEmpty(targetEmail))
            {
                var acc = _sampleAccounts.FirstOrDefault(a => a.Email == targetEmail);
                if (acc != null)
                {
                    acc.State = acc.State == "Activate" ? "Lock" : "Activate";
                }
            }

            // Lọc lại dữ liệu
            var filtered = _sampleAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                filtered = filtered.Where(a =>
                    a.Name.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(model.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(model.RoleFilter) && model.RoleFilter != "All")
            {
                filtered = filtered.Where(a => a.Role == model.RoleFilter);
            }

            model.Accounts = filtered.ToList();
            return View(model);
        }
    }
}
