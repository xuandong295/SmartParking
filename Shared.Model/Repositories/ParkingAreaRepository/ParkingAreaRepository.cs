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

namespace Shared.Model.Repositories.ParkingAreaRepository
{
    public class ParkingAreaRepository : BaseRepository<tblParkingArea>, IParkingAreaRepository
    {
        private new readonly DataContext DataContext;
        IPersistenceFactory PersistenceFactory;
        public ParkingAreaRepository(DataContext dataContext, IPersistenceFactory persistenceFactory) : base(dataContext)
        {
            PersistenceFactory = persistenceFactory;
            DataContext = dataContext;
        }
        public async Task<List<tblParkingArea>> GetAllParkingAreasAsync()
        {
            var allParkingArea = await DataContext.tblParkingArea.ToListAsync();
            return allParkingArea;
        }
        public async Task<tblParkingArea> GetParkingAreaAsync(string areaId)
        {
            var parkingArea = await DataContext.tblParkingArea.Where(o => o.Id == areaId).FirstOrDefaultAsync();
            return parkingArea;
        }
        public async Task<InternalAPIResponseCode> UpdateParkingSpaceAsync(string id, int current)
        {
            try
            {
                var parkingArea = await GetParkingAreaAsync(id);
                parkingArea.Current = current;
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
