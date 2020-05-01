using System;

namespace Bot.Interfaces
{
    public interface IScheduleManager
    {
        bool SetUpTimer(TimeSpan? alertTime, DateTime? alertDate, Action callback);
        void StopTimer();
    }
}
