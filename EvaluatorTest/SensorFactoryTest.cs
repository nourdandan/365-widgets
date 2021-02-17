using System;
using System.Collections.Generic;
using System.Text;
using Evaluator.Infrastructure;
using Xunit;

namespace EvaluatorTest
{
    public class SensorFactoryTest
    {
        const string envVariables = "reference 70.0 45.0 6";
        SensorFactory Factory = new SensorFactory();

        [Theory]
        [InlineData("")]
        [InlineData("reference 70.0 45.0 a")]
        [InlineData("reference a 45.0 6")]
        [InlineData("reference a 45.0")]
        [InlineData("reference 70 45.0 0.0")]
        public  void TestSensorFactory_WillThrowException(string envRef)
        {           
            Assert.Throws<Exception>(() => Factory.Setup(envRef));
        }



        [Theory]
        [InlineData("thermometer","name-1")]
        [InlineData("humidity","name-1")]
        [InlineData("monoxide","name-1")]
        public void TestSensorFactory_CreateOk(string type, string name)
        {
            var sensorInfo = new KeyValuePair<string, string>(name,type);
            Factory.Setup(envVariables);
            Factory.Create(sensorInfo);
            Assert.NotNull(Factory.Create(sensorInfo));
        }

        [Theory]
        [InlineData("unrecognized", "name-1")]
        [InlineData("", "name-1")]
        public void TestSensorFactory_Create_NullReturn(string type, string name)
        {
            var sensorInfo = new KeyValuePair<string, string>(name, type);
            Factory.Setup(envVariables);
            Factory.Create(sensorInfo);
            Assert.Null(Factory.Create(sensorInfo));
        }


        [Theory]
        [InlineData("thermometer", "name-1")]
        public void TestSensorEval_Thermo(string type, string name)
        {
            var sensorInfo = new KeyValuePair<string, string>(name, type);
            Factory.Setup(envVariables);
            var sensor =  Factory.Create(sensorInfo);
            AddData(sensor, thermoData);
            sensor.Evaluate();
            Assert.Equal("ultra precise", sensor.Status);
        }


        [Theory]
        [InlineData(@"2007-04-05T22:04 45.2
2007-04-05T22:05 45.3
2007-04-05T22:06 45.1", "keep")]
        [InlineData(@"2007-04-05T22:04 44.4
2007-04-05T22:05 43.9
2007-04-05T22:06 44.9
2007-04-05T22:07 43.8
2007-04-05T22:08 42.1", "discard")]
        public void TestSensorEval_Humidity(string data, string status)
        {
            var sensorInfo = new KeyValuePair<string, string>("hum-1", "humidity");
            Factory.Setup(envVariables);
            var sensor = Factory.Create(sensorInfo);
            AddData(sensor, data);
            sensor.Evaluate();
            Assert.Equal(status, sensor.Status);
        }

        [Theory]
        [InlineData(@"2007-04-05T22:04 5
2007-04-05T22:05 7
2007-04-05T22:06 9", "keep")]
        [InlineData(@"2007-04-05T22:04 2
2007-04-05T22:05 4
2007-04-05T22:06 10
2007-04-05T22:07 8
2007-04-05T22:08 6", "discard")]
        public void TestSensorEval_Mono(string data, string status)
        {
            var sensorInfo = new KeyValuePair<string, string>("mono-1","monoxide" );
            Factory.Setup(envVariables);
            var sensor = Factory.Create(sensorInfo);
            AddData(sensor, data);
            sensor.Evaluate();
            Assert.Equal(status, sensor.Status);
        }

        void AddData(ISensor sensor,string data)
        {
            foreach(var dataLine in data.Split("\r\n"))
            {
                sensor.Add(dataLine.SplitLine()[1]);
            }
        }

        string thermoData = @"2007-04-05T22:01 69.5
2007-04-05T22:02 70.1
2007-04-05T22:03 71.3
2007-04-05T22:04 71.5
2007-04-05T22:05 69.8";

        string humidData = @"2007-04-05T22:04 45.2
2007-04-05T22:05 45.3
2007-04-05T22:06 45.1";

        string carbMonoData = @"2007-04-05T22:04 2
2007-04-05T22:05 4
2007-04-05T22:06 10
2007-04-05T22:07 8
2007-04-05T22:08 6";
    }
}
