using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.Model
{
    public class tblParkingArea
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public int Current { get; set; }
        public int Maximum { get; set; }
    }
}
