using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Evaluator.Infrastructure
{
    public static class Extensions
    {
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                double avg = values.Average();

                double sum = values.Sum(d => (d - avg) * (d - avg));

                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        static string Consistant(Match match) => match.Value.ToLowerInvariant().Trim();
        static string Consistant(string value) => value.ToLowerInvariant().Trim();

        static RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        /// <summary>
        /// Matches pattern
        /// uses consistant Match Options and string result standard
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="pattern"></param>
        /// <returns>list of matched results</returns>
        public static List<string> GetMatchesPattern(this string logFile, string pattern)
        {
            Regex regexRef = new Regex(pattern, options);
            return regexRef.Matches(logFile).Select(x => Consistant(x)).ToList();
        }

        /// <summary>
        /// Matches pattern
        /// uses consistant Match Options and string result standard
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="pattern"></param>
        /// <returns>returns first result</returns>
        public static string GetMatchPattern(this string logFile, string pattern)
        {
            Regex regexRef = new Regex(pattern, options);
            return Consistant(regexRef.Match(logFile));
        }

        /// <summary>
        /// Uses a pattern as a delimiter to split
        /// skips first item in array as its not needed
        /// uses consistant Match Options and string result standard
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="pattern"></param>
        /// <returns>list with dilimiters and info in between each one</returns>
        public static List<string> GetSplittedOnPattern(this string logFile, string pattern)
        {
            Regex regexRef = new Regex(pattern, options);
            return regexRef.Split(logFile)
                .Select(x => Consistant(x))
                .Skip(1)
                .ToList();
        }

        public static KeyValuePair<string, string>? SensorInfoExtract(this string sensorInfo)
        {
            if (string.IsNullOrEmpty(sensorInfo)) return null;
            var info = sensorInfo.SplitLine();
            return info.Length < 2 ? null : (KeyValuePair<string, string>?)new KeyValuePair<string, string>(info[1], info[0]);
        }

        public static bool IsSensor(this ISensor sensor,string sensorFound)
        {
            var details  = sensorFound.Split(" ");
            return sensor.Name.StartsWith(details[1]) && sensor.Type.ToString() == details[0];
        }

        public static string[] SplitLine(this string dataLine)
        {
            return dataLine.Trim().Split(" ");
        }
    }
}
