using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext context, [FromBody] User model)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> GetById([FromServices]DataContext context, int id)
        {
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            return user;
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put([FromServices] DataContext context, [FromBody] User model, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado." });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível editar o usuário" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post([FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (context.Users.Where(x => x.Username == model.Username).FirstOrDefault() != null)
                    return BadRequest(new { message = "Usuário já cadastrado." });

                model.Role = "employee";
                context.Users.Add(model);

                model.Password = "";
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete([FromServices] DataContext context, int id)
        {
            try
            {
                var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
                if (user == null)
                    return BadRequest(new { message = "Usuário não encontrado." });

                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return user;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível deletar o usuário" });
            }
        }
    }
}
