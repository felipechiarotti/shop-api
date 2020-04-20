using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    // https:localhost:5001/categories
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            return Ok(await context.Categories.AsNoTracking().ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model,
            [FromServices]DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices]DataContext context)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (id != model.Id)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível alterar o registro" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<string>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}
