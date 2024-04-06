﻿using Manager.Domain.Common;

namespace Manager.Domain.Entity
{
    public class Player : BaseEntity
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}