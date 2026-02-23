using System;
using System.Globalization;
using Hangfire.Common;
using Hangfire.States;

namespace Madev.Utils.Infrastructure.Hangfire.Attributes
{

    public class AllowedExecutionTimeRangeAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly TimeSpan _timeFrom;
        private readonly TimeSpan _timeTo;

        /// <summary>
        /// Allows to run tasks only at a time interval
        /// </summary>
        /// <param name="timeFrom">Time from (HH:mm:ss)</param>
        /// <param name="timeTo">Time to (HH:mm:ss)</param>
        public AllowedExecutionTimeRangeAttribute(string timeFrom, string timeTo)
        {
            _timeFrom = DateTime.ParseExact(timeFrom, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
            _timeTo = DateTime.ParseExact(timeTo, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
        }

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CurrentState != EnqueuedState.StateName) return;

            var state = context.Connection.GetStateData(context.BackgroundJob.Id);
            if (state == null) return; // just in case

            var dateTimeNow = GetTimeInCentralEuropeStandardTime(DateTime.Now);

            if (JobIsAllowedInActualTime(dateTimeNow) == false)
            {
                context.CandidateState = new FailedState(new ArgumentOutOfRangeException($"It is not allowed to perform the task at {dateTimeNow}"))
                {
                    Reason = $"It is not allowed to perform the task at {dateTimeNow}"
                };
            }
        }

        public bool JobIsAllowedInActualTime(TimeSpan now)
        {
            if (_timeFrom == _timeTo)
                throw new Exception("Duration cannot be 0h0m0s");

            // if range is over midnight (from: 23:00:00 to: 01:00:00 duration: 2h)
            if (_timeFrom > _timeTo)
            {
                if ((now >= _timeFrom) || (now <= _timeTo))
                {
                    return true;
                }
            }

            // if range is over day (from: 01:00:00 to: 23:00:00 duration: 22h)
            if (_timeFrom < _timeTo)
            {
                if ((now >= _timeFrom) && (now <= _timeTo))
                {
                    return true;
                }
            }

            return false;
        }

        public TimeSpan GetTimeInCentralEuropeStandardTime(DateTime dateTime)
        {
            TimeZoneInfo infotime = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, infotime.Id).TimeOfDay;
        }
    }
}