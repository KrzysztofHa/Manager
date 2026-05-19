using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.TypeOfplayPlaySystem
{
    public class TypeOfPlaySystem
    {
        public List<string> ListTypeOfPlaySystems { get; set; }

        public TypeOfPlaySystem()
        {
            ListTypeOfPlaySystems = new List<string>() { "Group", "2KO" };
        }
    }
}