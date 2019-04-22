using System;
using System.Collections.Generic;
using System.Text;

namespace UltraGiveawayBot
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
