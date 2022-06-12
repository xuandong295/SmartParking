using Shared.Model.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.ParkingAreaRepository
{
    public interface IParkingAreaRepository
    {
        public Task<InternalAPIResponseCode> UpdateParkingSpaceAsync(string id, int current);
        public Task<tblParkingArea> GetParkingAreaAsync(string id);
        public Task<List<tblParkingArea>> GetAllParkingAreasAsync();


    }
}
