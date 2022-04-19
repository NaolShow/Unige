// <copyright file="TimeSlot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OgeSharp.Schedule
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Stores informations about a time slot
    /// </summary>
    public class TimeSlot
    {
        /// <summary>
        /// Regex that extracts the ressource type
        /// </summary>
        private static readonly Regex RessourceTypeRegex = new(@"[\w]{2}\.[\w]{2}");

        internal TimeSlot(dynamic slot)
        {
            // Parse the start and end time
            Start = DateTime.Parse(slot["start"]);
            End = DateTime.Parse(slot["end"]);
            Duration = End - Start;

            // Get the teacher and room
            string[] description = ((string) slot["title"]).Split('\n');
            Teacher = description[2].Trim();
            Room = description[1].Trim();

            // Get the title
            FullTitle = ((string) slot["description"]).Trim();
        }


        /// <summary>
        /// Gets dateTime at which the class starts
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// Gets dateTime at which the class ends
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Gets timeSpan that represents the duration of the class
        /// </summary>
        public TimeSpan Duration { get; }


        /// <summary>
        /// Gets teacher's full name
        /// </summary>
        public string Teacher { get; }

        /// <summary>
        /// Gets room number/id
        /// </summary>
        public string Room { get; }

        /// <summary>
        /// Gets full title of the class
        /// </summary>
        public string FullTitle { get; }

        /// <summary>
        /// Associated ressource of the class
        /// </summary>
        public string Ressource => RessourceTypeRegex.Match(FullTitle)?.Groups[0].Value;
    }
}