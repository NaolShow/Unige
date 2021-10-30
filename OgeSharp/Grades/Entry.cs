using System.Collections.Generic;

namespace OgeSharp {

    /// <summary>
    /// Represents an entry for the grades<br/>
    /// (This can be a folder of grades as well as a grade see <see cref="IsFolder"/>)
    /// </summary>
    public class Entry {

        /// <summary>
        /// Determines the normalized max grade value
        /// </summary>
        public const double NormalizedValue = 20;

        /// <summary>
        /// Creates a folder entry with a name and coefficient
        /// </summary>
        internal Entry(string name, double coefficient) {

            // Save the name and coefficient
            Name = name;
            Coefficient = coefficient;

            // Initialize the child entries list
            Entries = new List<Entry>();

        }

        /// <summary>
        /// Creates a grade entry with a grade, max grade and coefficient
        /// </summary>
        internal Entry(double grade, double maxGrade, double coefficient) {

            // Save the grade, maxGrade and coefficient
            Grade = grade;
            MaxGrade = maxGrade;
            Coefficient = coefficient;

        }

        #region Folder

        /// <summary>
        /// Determines if the entry is a folder for other entries
        /// </summary>
        public bool IsFolder => Name != null;

        /// <summary>
        /// List the child of the Entry (in case the current entry is a folder)<br/>
        /// (Only available if the entry IS a folder, else the list equals null)
        /// </summary>
        public List<Entry> Entries { get; }

        #endregion

        /// <summary>
        /// Determines the name of the entry
        /// </summary>
        public string Name;

        /// <summary>
        /// Represents the normalized entry's grade to 20 (its grade is adjusted to have a maximum of 20)<br/>
        /// (Only available if the entry is NOT a folder)
        /// </summary>
        public double NormalizedGrade => _Grade * NormalizedValue / MaxGrade;

        private double _Grade = double.NaN;

        /// <summary>
        /// Gets the grade if the entry is one OR<br/>
        /// Gets the average of the child grades if the entry is a folder
        /// </summary>
        public double Grade {

            get {

                // If it's not a folder then simply return the value
                if (!IsFolder) return _Grade;

                // If the folder already have a cached grade
                if (!double.IsNaN(_Grade)) return _Grade;

                // Loop recursively through the entries and return the grade
                return GetGradesRecursively();

            }
            set => _Grade = value;

        }
        public double MaxGrade { get; }
        public double Coefficient { get; }

        /// <summary>
        /// Returns the grade if the entry is a grade AND<br/>
        /// Calculates the average of all the child grades if it's a folder
        /// </summary>
        internal double GetGradesRecursively() {

            // If it's not a folder
            if (!IsFolder) return NormalizedGrade;

            // If the folder do not have any grade
            if (Entries.Count <= 0) return double.NaN;

            // Initialize the grades sum and count (taking in account the coefficients)
            double gradesSum = 0;
            double gradesCount = 0;

            // Loop through the child entries
            foreach (Entry entry in Entries) {

                // Get it's grade or calculate it
                double grade = entry.GetGradesRecursively();

                // If the grade is NaN (the folder have no grade)
                if (double.IsNaN(grade)) continue;

                // Increase the grades count
                gradesSum += grade * entry.Coefficient;
                gradesCount += entry.Coefficient;

            }

            // Cache the average of the grades
            _Grade = gradesSum / gradesCount;
            return _Grade;

        }

    }

}
