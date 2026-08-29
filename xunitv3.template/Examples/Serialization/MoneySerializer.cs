using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit.Sdk;
using xunitv3.template.Examples.Serialization;

[assembly: RegisterXunitSerializer(typeof(MoneySerializer), typeof(Money))]

namespace xunitv3.template.Examples.Serialization;

/// <summary>External serializer (<see cref="IXunitSerializer"/>) registered assembly-wide via <see cref="RegisterXunitSerializerAttribute"/>.</summary>
public sealed class MoneySerializer : IXunitSerializer
{
    public const char Separator = ' ';

    private const string UnsupportedTypeFormat = "Type {0} is not supported";
    private const string MalformedValueFormat = "Serialized value '{0}' is malformed";

    public object Deserialize(Type type, string serializedValue)
    {
        Ensure(type);

        return serializedValue.Split(Separator) is [var amount, var currency]
            ? new Money(decimal.Parse(amount, CultureInfo.InvariantCulture), currency)
            : throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MalformedValueFormat, serializedValue), nameof(serializedValue));
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = type == typeof(Money) && value is Money
            ? null
            : string.Format(CultureInfo.InvariantCulture, UnsupportedTypeFormat, type.FullName);

        return failureReason is null;
    }

    public string Serialize(object value)
    {
        Ensure(value.GetType());

        var money = (Money)value;
        return string.Create(CultureInfo.InvariantCulture, $"{money.Amount}{Separator}{money.Currency}");
    }

    private static void Ensure(Type type)
    {
        if (type != typeof(Money))
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, UnsupportedTypeFormat, type.FullName), nameof(type));
        }
    }
}
