using Shared.Model.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<InternalAPIResponseCode> RegisterAsync(tblUser user);
        Task<tblUser> LoginAsync(string userName, string password);
        Task<InternalAPIResponseCode> CaculateParkingFee(string licensePlate);
    }
}
