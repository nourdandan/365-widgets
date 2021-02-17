using Evaluator.Modules;
using System;
using System.Collections.Generic;

namespace Evaluator.Infrastructure
{
    public class SensorFactory : ISensorFactory
    {
        private double envTemp;
        private double envHumidity;
        private int envMonoxide;
        private const string ReferenceNumbersPattern = @"\d+(\.\d{1,2})?";

        public void Setup(string envRef)
        {
            var envKnownValues = envRef.GetMatchesPattern(ReferenceNumbersPattern);
            if (envKnownValues.Count < 3) throw new Exception($"Env info : {envRef} does not match our pattern {ReferenceNumbersPattern}");
            if (!double.TryParse(envKnownValues[0], out envTemp) ||
                !double.TryParse(envKnownValues[1], out envHumidity) ||
                !int.TryParse(envKnownValues[2], out envMonoxide))
            {
                throw new Exception($"Error happened while trying to parse environment values: {envRef}");
            }
        }

       public ISensor Create(KeyValuePair<string, string>? sensorIdentifier)
        {
            if (!sensorIdentifier.HasValue)
                throw new Exception("Not enough sensor information to create one");
            Enum.TryParse(typeof(SensorType), sensorIdentifier.Value.Value, out object sensorType);
            switch (sensorType)
            {
                case SensorType.thermometer:
                    return new Thermometer(envTemp, sensorIdentifier.Value.Key);
                case SensorType.humidity:
                    return new Humidity(envHumidity, sensorIdentifier.Value.Key);
                case SensorType.monoxide:
                    return new CarbMonoxide(envMonoxide, sensorIdentifier.Value.Key);
                default:
                    return null;
            }
        }

    }
}
