using IntermediateAPI.Models;
using IntermediateAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeRealmController : ControllerBase
    {
        private readonly HomeRealmService homerealm;
        public HomeRealmController(HomeRealmService _homeRealm)
        {
            homerealm = _homeRealm;
        }
        [HttpPost]
        public async Task<IActionResult> Find(HomeRealmInput input)
        {
            var result =  await homerealm.GetDomain(input.UserDomain);
            return Ok(result);
        }
    }
}
