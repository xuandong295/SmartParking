using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.ElasticSearchModel
{
    public class Car
    {
        public string Id { get; set; }
        public string FrontImageLink { get; set; }
        public string BackImageLink { get; set; }
        public string LicensePlateNumber { get; set; }
        public string ParkingAreaId { get; set; }
        public long TimeIn { get; set; }
        public long TimeOut { get; set; }
        public int Status { get; set; }
    }
}
