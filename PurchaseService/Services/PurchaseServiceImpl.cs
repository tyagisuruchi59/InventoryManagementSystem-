// StockPro Inventory Management System
// Service: Purchase Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Business logic for purchase order lifecycle management

using PurchaseService.DTOs;
using PurchaseService.Models;
using PurchaseService.Repositories;

namespace PurchaseService.Services
{
    public class PurchaseServiceImpl : IPurchaseService
    {
        private readonly IPurchaseRepository _repo;

        public PurchaseServiceImpl(IPurchaseRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<POResponseDto>> GetAllPOsAsync()
        {
            var pos = await _repo.GetAllAsync();
            return pos.Select(MapToDto).ToList();
        }

        public async Task<POResponseDto?> GetPOByIdAsync(int id)
        {
            var po = await _repo.GetByIdAsync(id);
            return po == null ? null : MapToDto(po);
        }

        public async Task<List<POResponseDto>> GetPOsBySupplierAsync(int supplierId)
        {
            var pos = await _repo.GetBySupplierAsync(supplierId);
            return pos.Select(MapToDto).ToList();
        }

        public async Task<List<POResponseDto>> GetPOsByWarehouseAsync(int warehouseId)
        {
            var pos = await _repo.GetByWarehouseAsync(warehouseId);
            return pos.Select(MapToDto).ToList();
        }

        public async Task<List<POResponseDto>> GetPOsByStatusAsync(string status)
        {
            var pos = await _repo.GetByStatusAsync(status);
            return pos.Select(MapToDto).ToList();
        }

        public async Task<List<POResponseDto>> GetPOsByCreatedByAsync(int userId)
        {
            var pos = await _repo.GetByCreatedByAsync(userId);
            return pos.Select(MapToDto).ToList();
        }

        public async Task<List<POResponseDto>> GetPOsByDateRangeAsync(DateTime from, DateTime to)
        {
            var pos = await _repo.GetByDateRangeAsync(from, to);
            return pos.Select(MapToDto).ToList();
        }

        // CREATE PO - starts as Draft
        public async Task<string> CreatePOAsync(CreatePODto dto)
        {
            var po = new PurchaseOrder
            {
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
                CreatedById = dto.CreatedById,
                Status = "Draft",
                ExpectedDate = dto.ExpectedDate,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                LineItems = dto.LineItems.Select(l => new POLineItem
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitCost = l.UnitCost,
                    ReceivedQty = 0
                }).ToList()
            };

            // Calculate total amount
            po.TotalAmount = po.LineItems.Sum(l => l.Quantity * l.UnitCost);

            await _repo.AddAsync(po);
            await _repo.SaveChangesAsync();
            return "Purchase order created successfully";
        }

        // UPDATE PO - only allowed in Draft status
        public async Task<bool> UpdatePOAsync(int id, UpdatePODto dto)
        {
            var po = await _repo.GetByIdAsync(id);
            if (po == null) return false;
            if (po.Status != "Draft") return false;

            po.ExpectedDate = dto.ExpectedDate;
            po.Notes = dto.Notes;
            po.ReferenceNumber = dto.ReferenceNumber;

            await _repo.UpdateAsync(po);
            await _repo.SaveChangesAsync();
            return true;
        }

        // APPROVE PO - moves from Draft/Pending to Approved
        public async Task<string> ApprovePOAsync(int id)
        {
            var po = await _repo.GetByIdAsync(id);
            if (po == null) return "PO not found";
            if (po.Status == "Cancelled") return "Cannot approve a cancelled PO";
            if (po.Status == "FullyReceived") return "PO already fully received";

            po.Status = "Approved";
            await _repo.UpdateAsync(po);
            await _repo.SaveChangesAsync();
            return "Purchase order approved successfully";
        }

        // RECEIVE GOODS - records quantities received per line item
        public async Task<string> ReceiveGoodsAsync(ReceiveGoodsDto dto)
        {
            var po = await _repo.GetByIdAsync(dto.PurchaseOrderId);
            if (po == null) return "PO not found";
            if (po.Status == "Cancelled") return "Cannot receive goods for cancelled PO";
            if (po.Status == "Draft") return "PO must be approved before receiving goods";

            foreach (var item in dto.ReceivedItems)
            {
                // Find matching line item
                var lineItem = po.LineItems.FirstOrDefault(l => l.Id == item.LineItemId);
                if (lineItem == null) continue;

                // Update received quantity
                lineItem.ReceivedQty += item.ReceivedQty;
            }

            // Check if all items fully received
            bool fullyReceived = po.LineItems.All(l => l.ReceivedQty >= l.Quantity);
            bool partiallyReceived = po.LineItems.Any(l => l.ReceivedQty > 0);

            if (fullyReceived)
            {
                po.Status = "FullyReceived";
                po.ReceivedDate = DateTime.UtcNow;
            }
            else if (partiallyReceived)
            {
                po.Status = "PartiallyReceived";
            }

            await _repo.UpdateAsync(po);
            await _repo.SaveChangesAsync();
            return "Goods received successfully";
        }

        // CANCEL PO - only Draft or Pending can be cancelled
        public async Task<string> CancelPOAsync(int id)
        {
            var po = await _repo.GetByIdAsync(id);
            if (po == null) return "PO not found";
            if (po.Status == "FullyReceived") return "Cannot cancel a fully received PO";

            po.Status = "Cancelled";
            await _repo.UpdateAsync(po);
            await _repo.SaveChangesAsync();
            return "Purchase order cancelled successfully";
        }

        // MAP PurchaseOrder to POResponseDto
        private POResponseDto MapToDto(PurchaseOrder po)
        {
            return new POResponseDto
            {
                Id = po.Id,
                SupplierId = po.SupplierId,
                WarehouseId = po.WarehouseId,
                CreatedById = po.CreatedById,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                ReceivedDate = po.ReceivedDate,
                ReferenceNumber = po.ReferenceNumber,
                Notes = po.Notes,
                CreatedAt = po.CreatedAt,
                LineItems = po.LineItems.Select(l => new LineItemResponseDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitCost = l.UnitCost,
                    TotalCost = l.Quantity * l.UnitCost,
                    ReceivedQty = l.ReceivedQty
                }).ToList()
            };
        }
    }
}