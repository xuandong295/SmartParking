using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Ops_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCarInformation(string id)
        {

            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.FAILED_CODE,
                Message = MessageAPIResponse.ERROR
            });
        }
    }
}
