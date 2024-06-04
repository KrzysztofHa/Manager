using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class PlayerMatch : BaseEntity
{
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
    public int IdClub { get; set; }

}
