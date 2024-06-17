﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    internal interface ITournamentsService
    {
        void AddNewTurnament();
        void StartTurnament();
        void EndTurnament();
    }
}
