﻿using Shared.Model.Entities.ElasticSearchModel;
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
        Task<InternalAPIResponseCode> GetCarInformation(string id);
        public Task InputCarIndex(Car car);
        public Task<List<Car>> GetAllCarParkingOnDate(string date);
        
    }
}
