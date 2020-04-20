using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employee = new User { Id = 1, Username = "robin", Password = "12345", Role = "employee" };
            var manager = new User { Id = 2, Username = "batman", Password = "12345", Role = "manager" };

            var category = new Category { Id = 1, Title = "Informatica" };
            var product = new Product { Id = 1, Category = category, Title = "Mouse", Price = 299.0, Description = "Mouse Gamer" };

            context.Users.Add(employee);
            context.Users.Add(manager);

            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados"
            });
        }

    }
}
