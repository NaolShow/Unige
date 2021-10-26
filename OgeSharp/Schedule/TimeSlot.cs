using System;
using System.Text.RegularExpressions;

namespace OgeSharp {

    /// <summary>
    /// Stores informations about a time slot
    /// </summary>
    public class TimeSlot {

        /// <summary>
        /// Regex that extracts the ressource type
        /// </summary>
        internal static Regex RessourceTypeRegex = new Regex(@"[\w]{2}\.[\w]{2}");

        internal TimeSlot(dynamic slot) {

            // Parse the start and end time
            Start = DateTime.Parse(slot["start"]);
            End = DateTime.Parse(slot["end"]);
            Duration = End - Start;

            // Get the teacher and room
            string[] description = ((string)slot["title"]).Split('\n');
            Teacher = description[2].Trim();
            Room = description[1].Trim();

            // Get the title
            FullTitle = ((string)slot["description"]).Trim();

        }

        #region Dates and Times

        /// <summary>
        /// DateTime at which the class starts
        /// </summary>
        public DateTime Start { get; }
        /// <summary>
        /// DateTime at which the class ends
        /// </summary>
        public DateTime End { get; }
        /// <summary>
        /// TimeSpan that represents the duration of the class
        /// </summary>
        public TimeSpan Duration { get; }

        #endregion

        /// <summary>
        /// Teacher's full name
        /// </summary>
        public string Teacher { get; }
        /// <summary>
        /// Room number/id
        /// </summary>
        public string Room { get; }

        /// <summary>
        /// Full title of the class
        /// </summary>
        public string FullTitle { get; }
        /// <summary>
        /// Associated ressource of the class
        /// </summary>
        public string Ressource => RessourceTypeRegex.Match(FullTitle)?.Groups[0].Value;

    }

}
