using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using Shared.Model.Repositories.CarInformationRepository;
using Shared.Model.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private IConfiguration _config;
        public UserController(DataContext context, IPersistenceFactory persistenceFactory, IUserRepository userRepository, IConfiguration config)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            UserRepository = userRepository;
            _config = config;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(tblUser user)
        {
            var currentUser = await UserRepository.RegisterAsync(user);
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = currentUser
            });
        }
        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var currentUser = await UserRepository.LoginAsync(username, password);
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
        [Route("fee")]
        public async Task<IActionResult> PaymentParkingFee(string licensePlate)
        {
            //await UserRepository.CaculateParkingFee(licensePlate);
            using (var messageDispatcher = PersistenceFactory.GetMessageDispatcher())
            {
                var scheduleMessage = new RabbitMQMessage
                {
                    Type = "cloudacc"
                    
                };

                using (var rabbitMqQueues = PersistenceFactory.GetAppConfig())
                {
                    messageDispatcher.Enqueue<RabbitMQMessage>("wpf-manage-queue", scheduleMessage);
                }
            }
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = null
            });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] tblUser login)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(tblUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        //new Claim("Id", userInfo.Id.ToString()),
                        new Claim("username", userInfo.UserName),
                        new Claim("roles", userInfo.Roles),
                        //new Claim("LisencePlateNumber", userInfo.LisencePlateNumber)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<tblUser> AuthenticateUser(tblUser login)
        {
            var currentUser = await _context.tblUser.Where(o => o.UserName == login.UserName && login.Password == o.Password).FirstOrDefaultAsync();
            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", "value3", "value4", "value5" };
        }
    }
}
