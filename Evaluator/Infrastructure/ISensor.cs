namespace Evaluator.Infrastructure
{
    public interface ISensor
    {
        string Status { get; set; }
        string Name { get; set; }
        SensorType Type { get; }
        void Add(string value);
        void Evaluate();
    }

    public enum SensorType
    {
        thermometer,
        humidity,
        monoxide
    }
}
