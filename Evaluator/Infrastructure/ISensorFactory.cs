using System.Collections.Generic;

namespace Evaluator.Infrastructure
{
    public interface ISensorFactory
    {
        void Setup(string envInfo);
        ISensor Create(KeyValuePair<string,string>? info);
    }
}
