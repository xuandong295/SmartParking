using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Model.Entities.Model
{
    public class Class
    {
        public int Classid { get; set; }
        public string Name { get; set; }
        public string Majors { get; set; }
    }
}
