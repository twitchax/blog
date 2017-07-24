using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreHello.Controllers
{
    [Route("api/[controller]")]
    public class FriendsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Helpers.Friends.Query);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var friend = await Helpers.Friends.Document(id).ReadAsync();
            return Ok(friend);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Friend friend)
        {
            var doc = await Helpers.Friends.Document().CreateAsync(friend);
            friend.Id = doc.Id;
            return Created(doc.Id.ToString(), friend);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]Friend friend)
        {
            await Helpers.Friends.Document(id).UpdateAsync(friend);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await Helpers.Friends.ClearAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await Helpers.Friends.Document(id).DeleteAsync();
            return Ok();
        }
    }
}
