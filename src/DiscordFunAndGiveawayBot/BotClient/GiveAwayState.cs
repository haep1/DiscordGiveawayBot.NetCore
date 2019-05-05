using System;
using System.Collections.Generic;
using System.Text;

namespace BotClient
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
