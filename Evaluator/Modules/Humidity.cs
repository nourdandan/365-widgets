using Evaluator.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Evaluator.Modules
{
    class Humidity : ISensor
    {
        private string _name;
        private readonly double _knownHumid;
        private List<double> humidityLevels = new List<double>();
        private string _status;

        public string Status { get => _status; set => _status = value; }
        public string Name { get => _name; set => _name = value; }
        public SensorType Type { get => SensorType.humidity; }

        internal Humidity(double humid, string name)
        {
            _name = name;
            _knownHumid = humid;
        }

        public void Add(string value)
        {
            double.TryParse(value, out double res);
            humidityLevels.Add(res);
        }

        public void Evaluate()
        {
            _status = humidityLevels.Where(x => OutOfRangeCheck(x)).Any() ? "discard" : "keep";

            bool OutOfRangeCheck(double value)  // keep if within 1 else discard
            {
                return _knownHumid - value > 1 || _knownHumid - value < -1;
            }
        }
    }
}
