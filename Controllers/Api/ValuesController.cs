using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreCustomIdentyJwtDemo.Controllers_Api
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public List<string> GetValues()
        {
            return new List<string>() { "Value1", "Value2" };
        }
    }
}