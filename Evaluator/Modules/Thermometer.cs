using Evaluator.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Evaluator
{
    class Thermometer:  ISensor
    {
        private string _name;
        private readonly double _knownTemp;
        private List<double> temp = new List<double>();
        private string _status;

        public string Status { get => _status; set => _status = value; }
        public string Name { get => _name; set => _name = value; }
        public SensorType Type { get => SensorType.thermometer; }

        internal Thermometer(double temp ,  string name)
        {
            _knownTemp = temp;
            _name = name;
        }
     
        public void Evaluate()
        {
            var stdDev = temp.StdDev();
            var mean = temp.Average();
            switch (_knownTemp - mean)
            {
                case double res when (res <= 0.5 || res >= -0.5) && stdDev < 3: // mean within 0.5 and standard deviation less than 3
                    _status = "ultra precise";
                    break;
                case double res when (res <= 0.5 || res >= -0.5) && stdDev < 5: // mean within 0.5 and standard deviation less than 5
                    _status = "very precise";
                    break;
                default:
                    _status = "precise"; // default
                    break;
            }
        }

        public void Add(string value) 
        {
            double.TryParse(value, out double res);
            temp.Add(res);
        }
    }
}
