// StockPro Inventory Management System
// Service: Alert Service | Controller: Alert
// Developer: Suru | April 2026
// Description: API endpoints for alert management

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlertService.DTOs;
using AlertService.Services;

namespace AlertService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        // POST /api/alert - Send single alert
        [HttpPost]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> SendAlert([FromBody] CreateAlertDto dto)
        {
            var result = await _alertService.SendAlertAsync(dto);
            return Ok(result);
        }

        // POST /api/alert/bulk - Send bulk alert
        [HttpPost("bulk")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SendBulk([FromBody] BulkAlertDto dto)
        {
            var result = await _alertService.SendBulkAlertAsync(dto);
            return Ok(result);
        }

        // GET /api/alert/recipient/{id} - Get alerts for recipient
        [HttpGet("recipient/{recipientId}")]
        [Authorize]
        public async Task<IActionResult> GetByRecipient(int recipientId)
        {
            var alerts = await _alertService.GetByRecipientAsync(recipientId);
            return Ok(alerts);
        }

        // GET /api/alert/unread/{recipientId} - Get unread alerts
        [HttpGet("unread/{recipientId}")]
        [Authorize]
        public async Task<IActionResult> GetUnread(int recipientId)
        {
            var alerts = await _alertService.GetUnreadAsync(recipientId);
            return Ok(alerts);
        }

        // GET /api/alert/unread/count/{recipientId} - Get unread count
        [HttpGet("unread/count/{recipientId}")]
        [Authorize]
        public async Task<IActionResult> GetUnreadCount(int recipientId)
        {
            var count = await _alertService.GetUnreadCountAsync(recipientId);
            return Ok(new { UnreadCount = count });
        }

        // GET /api/alert/unacknowledged - Get all unacknowledged alerts
        [HttpGet("unacknowledged")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetUnacknowledged()
        {
            var alerts = await _alertService.GetUnacknowledgedAsync();
            return Ok(alerts);
        }

        // GET /api/alert - Get all alerts (Admin only)
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _alertService.GetAllAsync();
            return Ok(alerts);
        }

        // PUT /api/alert/{id}/read - Mark alert as read
        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _alertService.MarkAsReadAsync(id);
            if (!result) return NotFound("Alert not found");
            return Ok("Alert marked as read");
        }

        // PUT /api/alert/read/all/{recipientId} - Mark all as read
        [HttpPut("read/all/{recipientId}")]
        [Authorize]
        public async Task<IActionResult> MarkAllRead(int recipientId)
        {
            await _alertService.MarkAllReadAsync(recipientId);
            return Ok("All alerts marked as read");
        }

        // PUT /api/alert/{id}/acknowledge - Acknowledge alert
        [HttpPut("{id}/acknowledge")]
        [Authorize]
        public async Task<IActionResult> Acknowledge(int id)
        {
            var result = await _alertService.AcknowledgeAsync(id);
            if (!result) return NotFound("Alert not found");
            return Ok("Alert acknowledged");
        }

        // DELETE /api/alert/{id} - Delete alert
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _alertService.DeleteAlertAsync(id);
            if (!result) return NotFound("Alert not found");
            return Ok("Alert deleted");
        }
    }
}