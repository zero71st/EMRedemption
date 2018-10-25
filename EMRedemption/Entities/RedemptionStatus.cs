using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public enum RedemptionStatus:int
    {
        New = 1,
        ProcessStock = 2,
        EmailSended = 3
    }
}
