// File: OutboundTaskService.cs

using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Model.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IOutboundTaskServices
    {
        Task<List<OutboundTask>> CreateOutboundTaskMultiProductAsync();
        Task<List<OutboundTask>> CreateOutboundTaskSingleMixProductAsync();
        Task<List<OutboundTask>> CreateOutboundTaskSingleProductAsync();
    }

    public class OutboundTaskService : IOutboundTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OutboundTaskService> _logger;

        public OutboundTaskService(ApplicationDbContext context, ILogger<OutboundTaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> RunAllOutboundTaskProcessingAsync()
        {
            int totalCreated = 0;
            while (true)
            {
                var created1 = await CreateOutboundTaskSingleProductAsync();
                var created2 = await CreateOutboundTaskSingleMixProductAsync();
                var created3 = await CreateOutboundTaskMultiProductAsync();

                int totalThisRound = created1.Count + created2.Count + created3.Count;
                totalCreated += totalThisRound;

                _logger.LogInformation($"[Processing Round] Đã tạo {totalThisRound} OutboundTask trong vòng lặp.");

                if (totalThisRound == 0)
                    break;
            }

            _logger.LogInformation($"✅ Tổng số OutboundTask đã xử lý: {totalCreated}");
            return totalCreated;
        }

        private bool IsValidSingleProductSKU(ProductSKU sku)
        {
            bool hasProductId = sku.ProductId != null;
            bool hasComboId = sku.ComboId != null;
            return hasProductId ^ hasComboId;
        }

        public async Task<List<OutboundTask>> CreateOutboundTaskSingleProductAsync()
        {
            var createdOutboundTasks = new List<OutboundTask>();

            while (true)
            {
                var eligibleOrders = await _context.ShopOrders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductSKUs)
                    .Where(o => o.Status == StatusOrder.Created && o.OrderItems.Count == 1)
                    .ToListAsync();

                eligibleOrders = eligibleOrders
                    .Where(o => IsValidSingleProductSKU(o.OrderItems.First().ProductSKUs))
                    .ToList();

                var groupedOrders = eligibleOrders
                    .GroupBy(o => o.OrderItems.First().ProductSKUsId)
                    .Where(g => g.Count() >= 1)
                    .ToList();

                if (!groupedOrders.Any()) break;

                bool anyCreated = false;

                foreach (var group in groupedOrders)
                {
                    var selectedOrders = group.Count() >= 8 ? group.Take(8).ToList()
                                           : group.Count() > 6 ? group.Take(6).ToList()
                                           : group.ToList();

                    var outboundTask = new OutboundTask
                    {
                        Id = Guid.NewGuid(),
                        CreateAt = DateTime.UtcNow,
                        Status = StatusOutbountTask.Incomplete,
                        OutboundTaskItems = new List<OutboundTaskItems>()
                    };

                    foreach (var order in selectedOrders)
                    {
                        var item = order.OrderItems.First();

                        var outboundTaskItem = new OutboundTaskItems
                        {
                            Id = Guid.NewGuid(),
                            ShopOrderId = order.ShopOrderId,
                            Status = StatusOutboundTaskItems.Created,
                            OutboundTaskItemDetails = new List<OutboundTaskItemDetails>
                            {
                                new OutboundTaskItemDetails
                                {
                                    Id = Guid.NewGuid(),
                                    Quantity = item.Quantity,
                                    ProductSKUId = item.ProductSKUsId
                                }
                            }
                        };

                        outboundTask.OutboundTaskItems.Add(outboundTaskItem);
                    }

                    _context.OutboundTasks.Add(outboundTask);
                    await _context.SaveChangesAsync();

                    foreach (var outboundTaskItem in outboundTask.OutboundTaskItems)
                    {
                        var pickTask = await CreatePickTaskAsync(outboundTaskItem);
                        if (!pickTask.PickTaskDetails.Any()) continue;
                        _context.PickTasks.Add(pickTask);
                    }

                    foreach (var order in selectedOrders)
                        order.Status = StatusOrder.Confirmed;

                    await _context.SaveChangesAsync();
                    createdOutboundTasks.Add(outboundTask);
                    anyCreated = true;
                }

                if (!anyCreated) break;
            }

            return createdOutboundTasks;
        }

        public async Task<List<OutboundTask>> CreateOutboundTaskSingleMixProductAsync()
        {
            var createdOutboundTasks = new List<OutboundTask>();

            while (true)
            {
                var eligibleOrders = await _context.ShopOrders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductSKUs)
                    .Where(o => o.Status == StatusOrder.Created && o.OrderItems.Count == 1)
                    .ToListAsync();

                eligibleOrders = eligibleOrders
                    .Where(o => o.OrderItems.First().ProductSKUs.ProductId == null
                             && o.OrderItems.First().ProductSKUs.ComboId != null)
                    .ToList();

                var groupedOrders = eligibleOrders
                    .GroupBy(o => o.OrderItems.First().ProductSKUsId)
                    .Where(g => g.Count() >= 1)
                    .ToList();

                if (!groupedOrders.Any()) break;

                bool anyCreated = false;

                foreach (var group in groupedOrders)
                {
                    var selectedOrders = group.Count() >= 8 ? group.Take(8).ToList()
                                           : group.Count() > 6 ? group.Take(6).ToList()
                                           : group.ToList();

                    var outboundTask = new OutboundTask
                    {
                        Id = Guid.NewGuid(),
                        CreateAt = DateTime.UtcNow,
                        Status = StatusOutbountTask.Incomplete,
                        OutboundTaskItems = new List<OutboundTaskItems>()
                    };

                    foreach (var order in selectedOrders)
                    {
                        var item = order.OrderItems.First();

                        var outboundTaskItem = new OutboundTaskItems
                        {
                            Id = Guid.NewGuid(),
                            ShopOrderId = order.ShopOrderId,
                            Status = StatusOutboundTaskItems.Created,
                            OutboundTaskItemDetails = new List<OutboundTaskItemDetails>
                            {
                                new OutboundTaskItemDetails
                                {
                                    Id = Guid.NewGuid(),
                                    Quantity = item.Quantity,
                                    ProductSKUId = item.ProductSKUsId
                                }
                            }
                        };

                        outboundTask.OutboundTaskItems.Add(outboundTaskItem);
                    }

                    _context.OutboundTasks.Add(outboundTask);
                    await _context.SaveChangesAsync();

                    foreach (var outboundTaskItem in outboundTask.OutboundTaskItems)
                    {
                        var pickTask = await CreatePickTaskAsync(outboundTaskItem);
                        if (!pickTask.PickTaskDetails.Any()) continue;
                        _context.PickTasks.Add(pickTask);
                    }

                    foreach (var order in selectedOrders)
                        order.Status = StatusOrder.Confirmed;

                    await _context.SaveChangesAsync();
                    createdOutboundTasks.Add(outboundTask);
                    anyCreated = true;
                }

                if (!anyCreated) break;
            }

            return createdOutboundTasks;
        }

        public async Task<List<OutboundTask>> CreateOutboundTaskMultiProductAsync()
        {
            var createdTasks = new List<OutboundTask>();

            while (true)
            {
                var eligibleOrders = await _context.ShopOrders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductSKUs)
                    .Where(o => o.Status == StatusOrder.Created && o.OrderItems.Count >= 2)
                    .ToListAsync();

                eligibleOrders = eligibleOrders
                    .Where(o => o.OrderItems.Select(oi => oi.ProductSKUsId).Distinct().Count() >= 2)
                    .ToList();

                if (!eligibleOrders.Any()) break;

                var selectedOrdersList = eligibleOrders.Chunk(8).ToList();
                bool anyCreated = false;

                foreach (var selectedOrders in selectedOrdersList)
                {
                    var outboundTask = new OutboundTask
                    {
                        Id = Guid.NewGuid(),
                        CreateAt = DateTime.UtcNow,
                        Status = StatusOutbountTask.Incomplete,
                        OutboundTaskItems = new List<OutboundTaskItems>()
                    };

                    var validOrders = new List<ShopOrder>();

                    foreach (var order in selectedOrders)
                    {
                        var groupedItems = order.OrderItems
                            .GroupBy(oi => oi.ProductSKUsId)
                            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

                        var outboundTaskItem = new OutboundTaskItems
                        {
                            Id = Guid.NewGuid(),
                            ShopOrderId = order.ShopOrderId,
                            Status = StatusOutboundTaskItems.Created,
                            OutboundTaskItemDetails = new List<OutboundTaskItemDetails>()
                        };

                        foreach (var kvp in groupedItems)
                        {
                            outboundTaskItem.OutboundTaskItemDetails.Add(new OutboundTaskItemDetails
                            {
                                Id = Guid.NewGuid(),
                                ProductSKUId = kvp.Key,
                                Quantity = kvp.Value
                            });
                        }

                        outboundTask.OutboundTaskItems.Add(outboundTaskItem);
                        validOrders.Add(order);
                    }

                    _context.OutboundTasks.Add(outboundTask);
                    await _context.SaveChangesAsync();

                    foreach (var outboundTaskItem in outboundTask.OutboundTaskItems)
                    {
                        var pickTask = await CreatePickTaskAsync(outboundTaskItem);
                        if (!pickTask.PickTaskDetails.Any()) continue;
                        _context.PickTasks.Add(pickTask);
                    }

                    foreach (var order in validOrders)
                        order.Status = StatusOrder.Confirmed;

                    await _context.SaveChangesAsync();
                    createdTasks.Add(outboundTask);
                    anyCreated = true;
                }

                if (!anyCreated) break;
            }

            return createdTasks;
        }

        private async Task<PickTasks> CreatePickTaskAsync(OutboundTaskItems outboundTaskItem)
        {
            var pickTask = new PickTasks
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                Status = StatusPickTask.Created,
                UsersTasksId = null,
                OutboundTaskItemId = outboundTaskItem.Id,
                OutboundTaskId = outboundTaskItem.OutboundTaskId,
                PickTaskDetails = new List<PickTaskDetails>()
            };

            var usedLocationIds = new HashSet<Guid>();
            int orderIndex = 1; // Bắt đầu đánh số thứ tự quét từ 1

            foreach (var detail in outboundTaskItem.OutboundTaskItemDetails)
            {
                var location = await _context.WarehouseLocations
                    .Include(l => l.WarehouseZones)
                    .Include(l => l.Inventory)
                    .Where(l => l.WarehouseZones.Name == "Outbound"
                                && l.Inventory != null
                                && l.Inventory.ProductSKUId == detail.ProductSKUId
                                && !usedLocationIds.Contains(l.Id))
                    .OrderByDescending(l => l.Inventory.StockQuantity)
                    .FirstOrDefaultAsync();

                if (location == null) continue;

                usedLocationIds.Add(location.Id);
                location.Inventory.QuantityBooked += detail.Quantity;
                location.Inventory.LastUpdated = DateTime.UtcNow;

                pickTask.PickTaskDetails.Add(new PickTaskDetails
                {
                    Id = Guid.NewGuid(),
                    ProductSKUId = detail.ProductSKUId,
                    Quantity = detail.Quantity,
                    WarehouseLocationId = location.Id,
                    PickTaskId = pickTask.Id,
                    QuantityPicked = 0,
                    OrderIndex = orderIndex++ // ⚠️ Gán thứ tự
                });
            }

            return pickTask;
        }

    }
}

