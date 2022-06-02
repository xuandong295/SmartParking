using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities
{
    public class tblParkingSpace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CurrentCar { get; set; }
        public int MaxCar { get; set; }
        public string Type { get; set; }
    }
}
