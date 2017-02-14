using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreHello.Controllers
{
    [Route("api/[controller]")]
    public class DbsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Helpers.DocumentDb.Query);
        }
    }
}
