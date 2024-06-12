using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class User: BaseEntity
{
    public string UserName { get; set; }
    public int? PlayerId {  get; set; }
    public string DisplayName { get; set; }
}
