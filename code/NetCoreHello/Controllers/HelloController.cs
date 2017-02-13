using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class HelloController : Controller
    {
        [HttpGet]
        [Route("api/hello/{name}")]
        public IActionResult SayHello(string name)
        {
            if(name == "Drumpf")
                return this.BadRequest("Drumpf?...Really?");

            var result = new { response = $"Hello, Awesome {name}‽" };
            
            return this.Ok(result);
        }
    }
}
