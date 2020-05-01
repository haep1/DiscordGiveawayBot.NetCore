using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Interfaces
{
    public enum GiveAwayState
    {
        None,
        SetGiveAwayTime,
        //SetCountGiveAways,
        SetAwardCultures,
        SetCountWinners,
        SetCodeword,
        SetAward,
        Initialized
    }
}
