using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.BusinessObject
{
    public enum GiveAwayState
    {
        None,
        SetGiveAwayTime,
        SetCountGiveAways,
        SetCountWinners,
        SetCodeword,
        SetAward,
        Initialized
    }
}
