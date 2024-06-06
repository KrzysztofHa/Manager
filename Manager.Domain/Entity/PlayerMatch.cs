using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class PlayerMatch : BaseEntity
{
    public DateTime StartMatch { get; set; }
    public DateTime EndMach { get; set; }
    public int IdClub { get; set; }
    public int IdFirstTeam { get; set; }
    public int IdSecondTeam { get; set;}


}
