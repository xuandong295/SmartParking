using Shared.Model.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.ParkingSpaceRepository
{
    public interface IParkingSpaceRepository
    {
        public Task<InternalAPIResponseCode> UpdateParkingSpaceAsync(string id, int state);
        public Task<List<tblParkingSpace>> GetAllParkingSpacesAsync();

    }
}
