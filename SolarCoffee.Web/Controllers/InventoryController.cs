using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        public InventoryController(IInventoryService inventory, ILogger<InventoryController> logger)
        {
            _inventoryService = inventory;
            _logger = logger;
        }

        [HttpGet("/api/inventory")]
        public ActionResult GetCurrentInventory()
        {
            _logger.LogInformation("Getting all inventory...");
            var inventory = _inventoryService.GetCurrentInventory()
                            .Select(pi => new ProductInventoryModel { 
                                Id = pi.Id,
                                IdealQuantity = pi.IdealQuantity,
                                QuantityOnHand = pi.QuantityOnHand,
                                Product = ProductMapper.SerializeProductModel(pi.Product)
                            })
                            .OrderBy(inv => inv.Product.Name)
                            .ToList();
            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogInformation($"Updating inventory for {shipment.ProductId} - Adjustment: {shipment.Adjustment}");
            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(id, adjustment);
            return Ok(inventory);           
        }

        [HttpGet("/api/inventory/snapshot")]
        public ActionResult GetSnapshotHistory()
        {
            _logger.LogInformation("Getting snapshot history");
            //YOU SHOULD CHECK OUT HOW THIS GROUP BY THING ACTUALLY WORKS NEXT TIME YOU RUN THIS
            try
            {
                List<ProductInventorySnapshot> snapshotHistory = _inventoryService.GetSnapshotHistory();

                //get distinct points in time a snapshot was collected
                List<DateTime> timelineMarkers = snapshotHistory.Select(t => t.SnapshotTime)
                                                     .Distinct()
                                                     .ToList();

                //get quantities grouped by id
                var snapshots = snapshotHistory
                                    .GroupBy(hist => hist.Product, hist => hist.QuantityOnHand,
                                    (key, g) => new ProductInventorySnapshotModel
                                    {
                                        ProductId = key.Id,
                                        QuantityOnHand = g.ToList()
                                    })
                                    .OrderBy(hist => hist.ProductId)
                                    .ToList();

                var viewModel = new SnapshotResponse
                {
                    Timeline = timelineMarkers,
                    ProductInventorySnapshots = snapshots
                };
                return Ok(viewModel);
            }
            catch(Exception e)
            {
                _logger.LogError("Error getting snapshot history.");
                _logger.LogError(e.StackTrace);
                return BadRequest("Error retrieving snapshot history");
            }
        }
    }
}
