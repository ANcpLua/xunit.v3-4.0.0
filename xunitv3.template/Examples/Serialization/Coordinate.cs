using System.Globalization;
using Xunit.Sdk;

namespace xunitv3.template.Examples.Serialization;

/// <summary>
/// Self-serializing theory argument (<see cref="IXunitSerializable"/>): each row becomes an
/// individually runnable test in Test Explorer, and <see cref="ToString"/> drives the display name.
/// </summary>
public sealed class Coordinate : IXunitSerializable
{
    private const string LatitudeKey = nameof(Latitude);
    private const string LongitudeKey = nameof(Longitude);
    private const string Format = "{0},{1}";

    // Required by the deserializer.
    public Coordinate()
    {
    }

    public Coordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        Latitude = info.GetValue<double>(LatitudeKey);
        Longitude = info.GetValue<double>(LongitudeKey);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(LatitudeKey, Latitude);
        info.AddValue(LongitudeKey, Longitude);
    }

    public override string ToString() => string.Format(CultureInfo.InvariantCulture, Format, Latitude, Longitude);
}
