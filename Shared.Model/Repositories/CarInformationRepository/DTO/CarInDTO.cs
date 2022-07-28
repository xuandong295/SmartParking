﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Repositories.CarInformationRepository.DTO
{
    public class CarInDTO
    {
        public string FrontImageLink { get; set; }
        public string BackImageLink { get; set; }
        public string LicensePlateNumber { get; set; }
        public string ParkingAreaId { get; set; }
    }
}
