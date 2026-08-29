namespace xunitv3.template.Examples.Serialization;

public sealed class SerializationTests
{
    private const string Euro = "EUR";
    private const string Dollar = "USD";
    private const decimal Price = 19.99m;
    private const decimal Free = 0m;
    private const double Latitude = 48.2082;
    private const double Longitude = 16.3738;

    public static IEnumerable<TheoryDataRow<Money>> Monies =>
    [
        new(new Money(Price, Euro)),
        new(new Money(Free, Dollar)),
    ];

    public static IEnumerable<TheoryDataRow<Coordinate>> Coordinates =>
    [
        new(new Coordinate(Latitude, Longitude)),
    ];

    [Theory, MemberData(nameof(Monies))]
    public void MoneyRoundTripsThroughExternalSerializer(Money money)
    {
        var serializer = new MoneySerializer();

        Assert.True(serializer.IsSerializable(typeof(Money), money, out var failureReason));
        Assert.Null(failureReason);
        Assert.Equal(money, serializer.Deserialize(typeof(Money), serializer.Serialize(money)));
    }

    [Fact]
    public void ExternalSerializerRejectsForeignTypes()
    {
        var serializer = new MoneySerializer();

        Assert.False(serializer.IsSerializable(typeof(Coordinate), new Coordinate(), out var failureReason));
        Assert.Contains(typeof(Coordinate).FullName!, failureReason);
    }

    // Pre-enumerated because Coordinate is IXunitSerializable; ToString() lands in the display name.
    [Theory, MemberData(nameof(Coordinates))]
    public void CoordinateToStringDrivesDisplayName(Coordinate coordinate)
    {
        Assert.Equal(Latitude, coordinate.Latitude);
        Assert.Contains(coordinate.ToString(), TestContext.Current.Test!.TestDisplayName);
    }
}
