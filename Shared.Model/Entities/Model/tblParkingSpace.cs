﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.Model
{
    public class tblParkingSpace
    {
        public string Id { get; set; }
        public string AreaId { get; set; }
        public string Position { get; set; }
        public int State { get; set; }
    }
}
