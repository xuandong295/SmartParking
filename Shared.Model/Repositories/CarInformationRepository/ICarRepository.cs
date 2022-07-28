using Shared.Model.Entities.ElasticSearchModel;
using Shared.Model.Repositories.CarInformationRepository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.CarInformationRepository
{
    public interface ICarRepository
    {
        Task<InternalAPIResponseCode> GetCarInformation(string licensePlate);
        Task<InternalAPIResponseCode> GetCarHistoryInformation(string licensePlate);

        public Task InputCarIndex(Car car);
        public Task<List<Car>> GetAllCarParkingOnDate(string date);
        public Task OutputCarIndex(Car car);


    }
}
