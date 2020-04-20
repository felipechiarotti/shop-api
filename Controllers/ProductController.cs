using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            try
            {
                var products = await context
                    .Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .ToListAsync();

                return products;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Houve um problema ao buscar os produtos" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(int id, [FromServices] DataContext context)
        {
            var product = await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product != null)
                return product;

            return NotFound(new { message = "Produto não encontrado" });
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext context)
        {
            try
            {
                var products = await context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .Where(x => x.CategoryId == id)
                    .ToListAsync();

                return products;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Houve um problema ao buscar o produto" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        [Route("")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Falha ao inserir o produto" });
            }
        }
    }
}
