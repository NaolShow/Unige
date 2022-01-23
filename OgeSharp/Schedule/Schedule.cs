using System;

namespace OgeSharp {

    /// <summary>
    /// Stores information about time slots
    /// </summary>
    public class Schedule {

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
        public TimeSpan Duration {

            get {

                // Initialize an empty time span
                TimeSpan span = new();

                // Loop through the time slots and add their duration
                foreach (TimeSlot slot in Slots) span = span.Add(slot.Duration);
                return span;

            }

        }

        internal Schedule(DateTime start, DateTime end, int timeSlotsCount) {

            // Initialize the time slot array
            Slots = new TimeSlot[timeSlotsCount];

            // Save the start and end dates
            Start = start;
            End = end;

        }

    }

}
