using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.EntityBase
{
    public class EntityLog : Entity
    {
        public string createOn { get; set; }

        [IgnoreDataMember]
        [ForeignKey(nameof(createBy))]

        public string createBy { get; set; }

        public string LastModifiedOn { get; set; }

        [IgnoreDataMember]
        [ForeignKey(nameof(lastmodifiedBy))]

        public string lastmodifiedBy { get; set; }
    }
}
