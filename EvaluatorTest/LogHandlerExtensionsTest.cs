using System;
using Xunit;
using Evaluator;
using Evaluator.Helpers;
using Evaluator.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Moq;
using Moq.Language;

namespace EvaluatorTest
{
    public class LogHandlerExtensionsTest
    {
        private const string ReferenceNumbersPattern = @"\d+(\.\d{1,2})?";
        private const string SensorsLogPattern = @"([a-z]+ [a-z]+-\d)";
        const string ReferencePattern = "^reference.*";

        [Theory]
        [InlineData("reference 70.0 45.0 6 \n thermometer temp - 1")]
        [InlineData("REFERENCE 70.0 45.0 6 \n thermometer temp - 1")]
        [InlineData("REFERENCE 70.0 45.0 6\r\n thermometer temp - 1")]
        [InlineData("REFERENCE 70.0 45.0 6")]
        public void TestPatternMatch_GetsExpected(string logFile)
        {
            Assert.Equal("reference 70.0 45.0 6", logFile.GetMatchPattern(ReferencePattern));
        }


        [Theory]
        [InlineData("reference 70.0 45.0 6", 3)]
        [InlineData("reference 70.0 45.0 d", 2)]
        [InlineData("reference d 45.0 d", 1)]
        public void TestPatternMatchCollection_GetsExpected(string logFile, int occurance)
        {
            var result = logFile.GetMatchesPattern(ReferenceNumbersPattern);
            Assert.Equal(occurance, result.Count);
        }

        [Theory]
        [InlineData(@"reference 70.0 45.0 6
thermometer temp-1
2007-04-05T22:00 72.4
humidity hum-1
2007-04-05T22:06 44.9", 4)]
        [InlineData(@"reference 70.0 45.0 6
thermometer temp-1
2007-04-05T22:00 72.4
humidity hum-1", 4)]
        [InlineData(@"reference 70.0 45.0 6
thermometer temp-1
2007-04-05T22:00 72.4
humidity hum-1
temp hum-1", 6)]
        [InlineData(@"reference 70.0 45.0 6
thermometer temp-1
2007-04-05T22:00 72.4
thermometer temp-1
2007-04-05T22:00 72.4
2007-04-05T22:00 72.4
humidity hum-1
2007-04-05T22:06 44.9", 6)]
        [InlineData(@"reference 70.0 45.0 6
thermometer temp-1
2007-04-05T22:00 72.4
thermometer temp-1
2007-04-05T22:00 72.4
2007-04-05T22:00 72.4
thermometer temp-1
2007-04-05T22:00 72.4
humidity hum-1
2007-04-05T22:06 44.9", 8)]
        public void TestPatternSplit_GetsExpected(string logFile, int occurance)
        {
            var result = logFile.GetSplittedOnPattern(SensorsLogPattern);
            Assert.Equal(occurance, result.Count());
        }



        [Fact]
        public void TestInfoExtract()
        {
            //Arrange
            var sensor1 = "thermometer temp-1";
            var sensor2 = "humid temp-2";
            var sensor3 = "carbon temp-3";
            var sensor4 = "carbon";
            var sensor5 = "";

            //Act
            var res = sensor1.SensorInfoExtract();
            var res2 = sensor2.SensorInfoExtract();
            var res3 = sensor3.SensorInfoExtract();
            var res4 = sensor4.SensorInfoExtract();
            var res5 = sensor5.SensorInfoExtract();

            //Assert
            Assert.Equal("temp-1", res.Value.Key);
            Assert.Equal("temp-2", res2.Value.Key);
            Assert.Equal("temp-3", res3.Value.Key);
            Assert.Equal("thermometer", res.Value.Value);
            Assert.Equal("humid", res2.Value.Value);
            Assert.Equal("carbon", res3.Value.Value);
            Assert.Null(res4);
            Assert.Null(res5);
        }


        [Theory]
        [InlineData("thermometer temp-1", "temp-1",true)]
        [InlineData("thermometer temp-2", "temp-2",true)]
        [InlineData("thermometer temp-1", "temp-2",false)]
        [InlineData("humidity temp-1", "temp-1",false)]
        public void TestIsCurrentSensor(string sensorInfo, string sensorName , bool isCurrentSensor)
        {
            SensorTest test = new SensorTest() { Name = sensorName };

            Assert.Equal(isCurrentSensor,test.IsSensor(sensorInfo));
        }


        class SensorTest : ISensor
        {
            public string Status { get; set; }
            public string Name { get; set; }

            public SensorType Type { get => SensorType.thermometer; }

            public void Add(string value)
            {
                throw new NotImplementedException();
            }

            public void Evaluate()
            {
                throw new NotImplementedException();
            }
        }
    }
}
