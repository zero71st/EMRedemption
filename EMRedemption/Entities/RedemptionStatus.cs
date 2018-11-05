﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public enum RedemptionStatus:int
    {
        Unprocess = 1,
        Processed = 2,
        EmailSuccess = 3,
        UnemailSuccess = 4,
    }
}
