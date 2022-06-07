using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using Shared.Model.Repositories.CarInformationRepository;
using Shared.Model.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Ops_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private IUserRepository UserRepository { get; set; }
        public UserController(DataContext context, IPersistenceFactory persistenceFactory, IUserRepository userRepository)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            UserRepository = userRepository;
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(string userName, string password)
        {
            var currentUser = await UserRepository.LoginAsync(userName, password);
            if (currentUser != null)
            {
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.SUCCESSED_CODE,
                    Message = MessageAPIResponse.OK,
                    Data = currentUser
                });
            }
            else
            {
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.FAILED_CODE,
                    Message = MessageAPIResponse.ACCESS_DENIED,
                    Data = null
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentParkingFee(string licensePlate)
        {
            await UserRepository.CaculateParkingFee(licensePlate);
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = null
            });
        }
    }
}
