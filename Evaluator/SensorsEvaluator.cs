using Evaluator.Helpers;
using System;

namespace Evaluator
{
    public static class SensorsEvaluator
    {
        public static string EvaluateLogFile(string logContentsStr)
        {
            try
            {
                var loghandler = new LogHandler(logContentsStr);
                loghandler.IterateThroughLogs();
                return loghandler.DumpData();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
