using System;
using System.Linq;

namespace OgeSharp.Schedule
{
    /// <summary>
    /// Stores information about time slots
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Array of the time slots of the schedule
        /// </summary>
        public TimeSlot[] Slots { get; }

        /// <summary>
        /// DateTime at which the schedule starts
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// DateTime at which the schedule ends
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Calculate the total duration of all the time slots
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                // Initialize an empty time span
                var span = default(TimeSpan);

                // Loop through the time slots and add their duration
                return Slots.Aggregate(span, (current, slot) => current.Add(slot.Duration));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="timeSlotsCount"></param>
        internal Schedule(DateTime start, DateTime end, int timeSlotsCount)
        {
            // Initialize the time slot array
            Slots = new TimeSlot[timeSlotsCount];

            // Save the start and end dates
            Start = start;
            End = end;
        }
    }
}