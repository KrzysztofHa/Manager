using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class Club : BaseEntity
{
    public string Name { get; set; }
    public int IdAddress { get; set; }
}
