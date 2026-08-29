namespace xunitv3.template.Examples.Serialization;

/// <summary>A type you do not own (or do not want to couple to xUnit): serialized externally by <see cref="MoneySerializer"/>.</summary>
public readonly record struct Money(decimal Amount, string Currency);
