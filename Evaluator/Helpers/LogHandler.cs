using Evaluator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Evaluator.Helpers
{
    public class LogHandler
    {
        const string referencePattern = "^reference.*";
        private const string ReferenceNumbersPattern = @"\d+(\.\d{1,2})?";
        private const string SensorsLogPattern = @"([a-z]+ [a-z]+-\d)";
        private readonly ISensorFactory sensorFactory;
        private readonly string _logFile;

        Regex regexRef = new Regex(referencePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Dictionary<string, ISensor> InitializedSensors = new Dictionary<string, ISensor>(); // used in an alternative solution
        List<ISensor> InitliazedSensors = new List<ISensor>();
        protected internal LogHandler(string logFile)
        {
            var reference = logFile.GetMatchPattern(referencePattern);
            sensorFactory = new SensorFactory();
            _logFile = logFile;
            sensorFactory.Setup(reference);
            //InitialiseSensors(logFile);   //this is an intitalizer used in an alternative solution
        }

        //loop solution of reading the sensor identifier with data
        protected internal void IterateThroughLogs()
        {
            foreach (var line in _logFile.Split("\r\n").Skip(1)) // can be parallelized  //skip reference initial line
            {

                if (Regex.IsMatch(line, SensorsLogPattern))
                {
                    //split identifier
                    //inititalize sensor using the identifier
                    var sensor = sensorFactory.Create(line.SensorInfoExtract());
                    if (sensor != null) InitliazedSensors.Add(sensor);
                }
                else
                {
                    //always add to the last added sensor in our collection
                    InitliazedSensors.LastOrDefault().Add(line.SplitLine()[1]);
                }
            }
        }

        protected internal string DumpData()
        {
            var sb = new StringBuilder();
            InitliazedSensors.ForEach(sen =>
            {
                sen.Evaluate();
                sb.AppendLine($"{sen.Name}: {sen.Status}");
            }
            );
            return sb.ToString();
        }

        //used in alternative regex with mapping solution
        protected void InitialiseSensors(string logFile)
        {
            //gets all sensors identifiers at once via pattern and initializez them
            logFile.GetMatchesPattern(SensorsLogPattern).ForEach(x =>
            {
                Add(x.SensorInfoExtract());
            });
        }

        //alternative regex solution to mapp exisiting identifiers with data
        protected void AddValues(Dictionary<string, ISensor> initializedSensors)
        {
            var splitLine = _logFile
                .GetSplittedOnPattern(SensorsLogPattern);

            splitLine
            .Where(splittedLine => Regex.IsMatch(splittedLine, SensorsLogPattern))
            .Select((n, i) => new { Value = n.Split(" ")[1], Index = i })
            .Select(x => MapSensorData(x.Index, x.Value))
            .ToList()
            .ForEach(x => { Action(x.Item1, x.Item2); });

            (string, ISensor) MapSensorData(int index, string key)
            {
                return (splitLine[index + 1], initializedSensors[key]);
            }

            void Action(string data, ISensor sensor)
            {
                var truncate = data.Split("\r\n");
                foreach (var elem in truncate)
                {
                    sensor.Add(elem.Split(" ")[1]);
                }
            }
        }

        //Add sensor whilst calling factory to create sensor using identifier
        protected void Add(KeyValuePair<string, string>? info)
        {
            if (info.HasValue)
            {
                InitializedSensors.Add(info.Value.Key, sensorFactory.Create(info));
            }
        }

    }
}
