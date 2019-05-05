using System;
using System.Threading;

namespace Scheduler
{
    public class ScheduleManager
    {
        private static TimeZoneInfo _berlinZone;

        public static TimeZoneInfo BerlinZone
        {
            get
            {
                if (_berlinZone == null)
                {
                    _berlinZone = GetBerlinZone();
                }
                return _berlinZone;
            }
        }

        private static TimeZoneInfo GetBerlinZone()
        {
            TimeZoneInfo.TransitionTime startTransition, endTransition;
            startTransition = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0),
                                                                              3, 5, DayOfWeek.Sunday);
            endTransition = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 3, 0, 0),
                                                                            10, 5, DayOfWeek.Sunday);
            TimeZoneInfo.AdjustmentRule adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1996, 10, 1), DateTime.MaxValue.Date, new TimeSpan(1, 0, 0), startTransition, endTransition);
            TimeZoneInfo.AdjustmentRule[] adjustments = { adjustment };

            TimeZoneInfo berlinZone = TimeZoneInfo.CreateCustomTimeZone("BerlinZone", new TimeSpan(1, 0, 0), "(UTC +01:00) Berlin", "Mitteleuropäische Zeit", "Mitteleuropäische Sommerzeit", adjustments);
            return berlinZone;
        }

        private static System.Threading.Timer _timer;
        public bool SetUpTimer(TimeSpan? alertTime, DateTime? alertDate, Action callback)
        {
            DateTime current = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), BerlinZone);
            Console.WriteLine("Current time: " + current.ToString("g"));
            TimeSpan timeToGo = new TimeSpan();

            if (alertTime != null)
            {
                timeToGo = alertTime.Value - current.TimeOfDay;
            }
            else if (alertDate != null)
            {
                timeToGo = alertDate.Value - current;
            }
            Console.WriteLine("Timespan to Giveaway: " + timeToGo);

            if (timeToGo < TimeSpan.Zero)
            {
                return false;//time already passed
            }
            _timer = new System.Threading.Timer(x =>
            {
                callback.Invoke();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

            return true;
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
