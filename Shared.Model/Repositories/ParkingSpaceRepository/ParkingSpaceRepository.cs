using Microsoft.EntityFrameworkCore;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using Shared.Model.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.ParkingSpaceRepository
{
    public class ParkingSpaceRepository : BaseRepository<tblParkingSpace>, IParkingSpaceRepository
    {
        private new readonly DataContext DataContext;
        IPersistenceFactory PersistenceFactory;
        public ParkingSpaceRepository(DataContext dataContext, IPersistenceFactory persistenceFactory) : base(dataContext)
        {
            PersistenceFactory = persistenceFactory;
            DataContext = dataContext;
        }
        public async Task<List<tblParkingSpace>> GetAllParkingSpacesAsync()
        {
            var allParkingSpace = await DataContext.tblParkingSpace.ToListAsync();
            return allParkingSpace;
        }
        public async Task<tblParkingSpace> GetParkingSpaceAsync(string id)
        {
            var parkingSpace = await DataContext.tblParkingSpace.Where(o=>o.Id == id).FirstOrDefaultAsync();
            return parkingSpace;
        }
        public async Task<InternalAPIResponseCode> UpdateParkingSpaceAsync(string id, int state)
        {
            try
            {
                var space = await DataContext.tblParkingSpace.Where(o => o.Id == id).FirstOrDefaultAsync();
                space.State = state;
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new InternalAPIResponseCode
                {
                    Code = APICodeResponse.FAILED_CODE,
                    Message = ex.Message,
                    Data = null
                };
            }

            return new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = null
            };
        }
    }
}
