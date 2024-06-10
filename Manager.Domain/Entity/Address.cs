using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class Address : BaseEntity
{
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Zip { get; set; }
}
