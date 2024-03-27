using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Common
{
    public class AuditableModel
    {
        public int CreateById {  get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedById { get; set; }
        public DateTime? ModifiedDateTime { get; set;}
    }
}
