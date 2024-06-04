using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Common;

public class PlayerMatch
{
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
    public int IdClub { get; set; }

}
