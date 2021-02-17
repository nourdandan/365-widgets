using Evaluator.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Evaluator.Modules
{
    class CarbMonoxide : ISensor
    {
        private string _name;
        private readonly int _knownCarbMono;
        private List<int> carbMonoLevels = new List<int>();
        private string _status;

        public string Status { get => _status; set => _status = value; }
        public string Name { get => _name; set => _name = value; }
        public SensorType Type { get => SensorType.humidity; }

        internal CarbMonoxide(int carbMono, string name)
        {
            _name = name;
            _knownCarbMono = carbMono;
        }

        public void Add(string value)
        {
            int.TryParse(value, out int res);
            carbMonoLevels.Add(res);
        }

        public void Evaluate()
        {
            _status = carbMonoLevels.Where(x => OutOfRangeCheck(x)).Any() ? "discard" : "keep";

            bool OutOfRangeCheck(double value) // keep if within 3 else discard
            {
                return _knownCarbMono - value > 3 || _knownCarbMono - value < -3;
            }
        }
    }
}
