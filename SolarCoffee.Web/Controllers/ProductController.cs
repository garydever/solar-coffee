using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        

        //The service layer (not the controller) connects to the database. The concerns of the controller should be just dealing with the requests and providing responses.

        [HttpGet("/api/product")]
        public ActionResult GetProduct()
        {
            _logger.LogInformation("Getting all products");
            var products = _productService.GetAllProducts();

            var productViewModels = products.Select(ProductMapper.SerializeProductModel);

            return Ok(productViewModels);
        }

        //This could be a Delete. Apparently Deletes are often used semanticly
        [HttpPatch("/api/product/{id}")]
        public ActionResult ArchiveProduct(int id)
        {
            _logger.LogInformation("Archiving product.");
            var archiveResult = _productService.ArchiveProduct(id);
            return Ok(archiveResult);
        }
    }
}
