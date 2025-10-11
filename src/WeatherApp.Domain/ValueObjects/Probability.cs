namespace WeatherApp.Domain.ValueObjects;

public sealed class Probability
{
    public double Value { get; }

    public Probability(double value)
    {
        if (value is < 0.0 or > 1.0)
            throw new ArgumentOutOfRangeException(nameof(value), "Probability must be between 0.0 and 1.0.");
        Value = value;
    }

    public static implicit operator double(Probability p) => p.Value;
    public static Probability FromPercent(double percent) => new(percent / 100.0);
    public double ToPercent() => Value * 100.0;
}
