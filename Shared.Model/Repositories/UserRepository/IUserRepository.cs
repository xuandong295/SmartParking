using Shared.Model.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> LoginAsync(string userName, string password);
        Task<long> CaculateParkingFee(string licensePlate);
    }
}
