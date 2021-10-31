using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnigeWebUtility;

namespace OgeSharp {

    public partial class Oge {

        internal Regex UINodeLevelRegex = new Regex(@"ui-node-level-([1-9]+)");
        internal Regex NotesRegex = new Regex(@"([0-9]+.[0-9]+)");

        // TODO: There is one case in which the system will not work:
        // If there is a grade that is not inside a folder
        // If that's the case the method will just crash.
        // It should never be the case, but if it is I'll need to fix it

        /// <summary>
        /// Returns a list of entries containing your grades
        /// </summary>
        public List<GradeEntry> GetGrades() {

            // Download the grades page source code and parse it as a document
            string gradesSource = Browser.Navigate(GradesUri).GetContent();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(gradesSource);

            // Get all the table rows
            HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//table//tbody//tr");

            // Initialize the entries and hierarchy
            List<GradeEntry> entries = new List<GradeEntry>();
            Stack<GradeEntry> stackEntries = new Stack<GradeEntry>();

            // Initialize the previous ui level
            int previousLevel = -1;

            // Loop through all the rows (backwards)
            foreach (HtmlNode row in rows) {

                // Get the row name and coefficient
                string rowName = row.SelectSingleNode(".//td[1]").InnerText;
                double rowCoefficient = double.Parse(row.SelectSingleNode(".//td[2]").InnerText, CultureInfo.InvariantCulture);

                // Get the row ui level
                int uiLevel = int.Parse(UINodeLevelRegex.Match(string.Join(' ', row.GetClasses())).Groups[1].Value);

                #region Folder processing

                // If it's a folder
                // By default the "aria-expanded" attribute is set to true for a subject folder
                if (row.GetAttributeValue("aria-expanded", true)) {

                    // If we got back in the hierarchy then get back in the entries
                    if (previousLevel > uiLevel) {

                        for (int level = 0; level < previousLevel - uiLevel; level++) stackEntries.Pop();

                    }

                    // Create the folder entry
                    GradeEntry folderEntry = new GradeEntry(rowName, rowCoefficient);

                    // If the folder have a parent then add it as a child
                    if (stackEntries.TryPeek(out GradeEntry parent)) {
                        parent.Entries.Add(folderEntry);
                    }
                    // Else add it directly to the entries list
                    else {
                        entries.Add(folderEntry);
                    }

                    // Push the entry in the hierarchy
                    stackEntries.Push(folderEntry);

                }

                #endregion
                #region Entry processing

                else {

                    // Initialize a row entry and add it 
                    GradeEntry rowEntry = new GradeEntry(rowName, rowCoefficient);
                    stackEntries.Peek().Entries.Add(rowEntry);

                    // Get the inner text of the grades column
                    string grades = row.SelectSingleNode(".//td[3]").InnerText;

                    // If the subject have some grades
                    if (!string.IsNullOrEmpty(grades)) {

                        // Loop through each lines of grades (looks like this):
                        // => gradeName [10.00 /10.0(1.0)   ](1.0)
                        // => gradeName  [4.50 /5.0(1.0)  8.00 /10.0(1.0)   ](1.0)
                        foreach (string line in grades.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)) {

                            // Get the grade name
                            string gradeName = line.Split('[')[0].TrimEnd();

                            // Get the grades and the coefficients
                            MatchCollection matches = NotesRegex.Matches(line);

                            // Initialize the grade entry
                            GradeEntry gradeEntry = new GradeEntry(gradeName, double.Parse(matches[matches.Count - 1].Groups[1].Value, CultureInfo.InvariantCulture));
                            rowEntry.Entries.Add(gradeEntry);

                            // Loop through the grades
                            for (int n = 0; n < matches.Count - 1; n += 3) {

                                // Get the grade, max grade and coefficient
                                double grade = double.Parse(matches[n].Groups[1].Value, CultureInfo.InvariantCulture);
                                double maxGrade = double.Parse(matches[n + 1].Groups[1].Value, CultureInfo.InvariantCulture);
                                double coefficient = double.Parse(matches[n + 2].Groups[1].Value, CultureInfo.InvariantCulture);

                                // Add a new grade entry
                                gradeEntry.Entries.Add(new GradeEntry(grade, maxGrade, coefficient));

                            }

                        }

                    }

                }

                #endregion

                // Save the current ui level as the previous one for the next iteration
                previousLevel = uiLevel;

            }
            return entries;

        }

    }

}
