using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Mongo.Models;
using Mongo.Services;

namespace Mongo.Controllers
{
    [ApiController]
    [Route("api/mongoproducts")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly IOptionsMonitor<ExampleSettings> _optionsMonitor;

        public ProductsController(
            IProductService products,
            IOptionsMonitor<ExampleSettings> optionsMonitor)
        {
            _productService = products;
            _optionsMonitor = optionsMonitor;

        }

        [HttpGet("settingsnew")]
        public async Task<object> GetSettings2()
        {
            return Results.Ok(new {
               optionMonitorTransient = _optionsMonitor.CurrentValue.One
           });
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get(int n = 20)
        {
            return await _productService.GetNAsync(n);
        }

        [HttpGet("{sku}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetBySku(string sku)
        {
            var product =  await _productService.GetBySkuAsync(sku);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            await _productService.CreateAsync(product);

            return CreatedAtRoute("GetProduct", new { sku = product.Sku }, product);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> Update(Product update)
        {
            var product = await _productService.UpdateAsync(update);

            return CreatedAtRoute("GetProduct", new { sku = product.Sku }, product);
        }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> Delete(string sku)
        {
            await _productService.DeleteAsync(sku);

            return Ok();
        }
    }
}
