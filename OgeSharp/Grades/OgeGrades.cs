using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnigeWebUtility;

namespace OgeSharp {

    public partial class Oge {

        /// <summary>
        /// Regex that let's me extract numbers from a string<br/>
        /// Example: "QCM [17.00 /20.0(1.0) 20.00 /20.0(1.0) 17.50 /20.0(1.0) ](1.0)"<br/>
        /// Will return all the numbers: 17.00, 20.0, 1.0, 20.00, 20.0, 1.0, 17.50, 20.0, 1.0, 1.0
        /// </summary>
        internal static readonly Regex NumbersRegex = new(@"([0-9]+.[0-9]+)");

        /// <summary>
        /// Regex that let's me extract coefficients (surrounded by parenthesis) out of some text<br/>
        /// Example: "TEST (10)" or "UE1.1 (15.00)"<br/>
        /// Will return: 10 or 15.00
        /// </summary>
        internal static readonly Regex CoefficientRegex = new(@"\(([0-9.]+)\)");

        /// <summary>
        /// Returns an entry containing all your grades<br/>
        /// For now it just get the selected semester, next update will let select one of them
        /// </summary>
        public GradeEntry GetGrades() {

            // Download the grades page source code and parse it as a document
            string gradesSource = Browser.Navigate(GradesUri).GetContent();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(gradesSource);

            // Initialize a root entry
            GradeEntry rootEntry = new GradeEntry("Root", 1);

            // Loop through all the UE divs
            foreach (HtmlNode ueNode in document.DocumentNode.SelectNodes("//div[@class='moy_UE']")) {

                // Get the UE name
                string ueText = ueNode.SelectSingleNode("./div[1]/span").InnerText;
                string ueName = ueText.Split('(')[0].TrimEnd();

                // Let's assume it's coefficient is 1 by default
                // => Only applies if the UE do not have any coefficient (never seen that, but just to be sure)
                double ueCoefficient = 1;

                // Try to get the coefficient out of the text and save it if it is valid
                Match match = CoefficientRegex.Match(ueText);
                if (match.Success) ueCoefficient = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                // Initialize an entry and add it to the parent
                GradeEntry ueEntry = new GradeEntry(ueName, ueCoefficient);
                rootEntry.Entries.Add(ueEntry);

                // Loop through both poles (ressource and SAE) and process both
                foreach (HtmlNode poleNode in ueNode.SelectNodes("./div[contains(@class, 'moy')]")) ProcessPole(poleNode, ueEntry);

            }
            return rootEntry;

        }

        private static void ProcessPole(HtmlNode poleNode, GradeEntry rootEntry) {

            // Get the pole name and coefficient
            string poleText = poleNode.SelectSingleNode(".//thead/tr/th[1]/span").InnerText;
            string poleName = poleText.Split('(')[0].TrimEnd();

            #region Coefficient

            // Initialize the coefficient
            double poleCoefficient;

            // Try to get the coefficient out of the text and save it if it is valid
            Match match = CoefficientRegex.Match(poleText);
            if (match.Success) poleCoefficient = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

            // Else, let's just guess it from the pole name
            // => I've just seen that it is the case for GEA
            else {

                // If the pole name contains "RESSOURCES" then set it's coefficient to 60
                if (poleName.Contains("RESSOURCES")) poleCoefficient = 60;
                // If the pole name contains "SAE" then set it's coefficient to 40
                else if (poleName.Contains("SAE")) poleCoefficient = 40;
                // Else this is a special case so let's just not count it and let the user report the issue
                else return;

            }

            #endregion

            // Initialize an entry and add it to the parent
            GradeEntry poleEntry = new GradeEntry(poleName, poleCoefficient);
            rootEntry.Entries.Add(poleEntry);

            // Loop through the grades categories and process them
            foreach (HtmlNode gradeCategoryNode in poleNode.SelectNodes(".//tbody/tr")) ProcessGradeCategory(gradeCategoryNode, poleEntry);

        }

        private static void ProcessGradeCategory(HtmlNode gradeCategoryNode, GradeEntry poleEntry) {

            // Get the category name
            // => Also remove the trailing colon
            string categoryName = gradeCategoryNode.SelectSingleNode("./td[1]").InnerText[0..^1];

            // If the category coefficient is not set, let's assume it's 1
            if (!double.TryParse(gradeCategoryNode.SelectSingleNode("./td[2]").InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double categoryCoefficient)) categoryCoefficient = 1;

            // Initialize an entry and add it to the parent
            GradeEntry categoryEntry = new GradeEntry(categoryName, categoryCoefficient);
            poleEntry.Entries.Add(categoryEntry);

            // Get the grades text and check if there is some grades
            string gradesText = gradeCategoryNode.SelectSingleNode("./td[3]").InnerText;
            if (!string.IsNullOrEmpty(gradesText)) {

                // Loop through each lines of grades (looks like this):
                // => gradeName [10.00 /10.0(1.0)   ](1.0)
                // => gradeName  [4.50 /5.0(1.0)  8.00 /10.0(1.0)   ](1.0)
                foreach (string line in gradesText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)) {

                    // Split the line into two parts
                    // => [0] : gradeName
                    // => [1] : grades
                    string[] splittedLine = line.Split('[');

                    // Get the grade name
                    string gradeName = splittedLine[0].TrimEnd();

                    // Get the grades and the coefficients
                    // => We only match the line without the grade name
                    // => Because if we have a number in the name it will be matched and will crash everything
                    MatchCollection matches = NumbersRegex.Matches(splittedLine[1]);

                    // Initialize the grade entry
                    GradeEntry gradeEntry = new GradeEntry(gradeName, double.Parse(matches[^1].Groups[1].Value, CultureInfo.InvariantCulture));
                    categoryEntry.Entries.Add(gradeEntry);

                    // Loop through the grades
                    for (int n = 0; n < matches.Count - 1; n += 3) {

                        // Get the grade, max grade and coefficient
                        double grade = double.Parse(matches[n].Groups[1].Value, CultureInfo.InvariantCulture);
                        double maxGrade = double.Parse(matches[n + 1].Groups[1].Value, CultureInfo.InvariantCulture);
                        double gradeCoefficient = double.Parse(matches[n + 2].Groups[1].Value, CultureInfo.InvariantCulture);

                        // Add a new grade entry
                        gradeEntry.Entries.Add(new GradeEntry(grade, maxGrade, gradeCoefficient));

                    }

                }

            }

        }

    }

}
