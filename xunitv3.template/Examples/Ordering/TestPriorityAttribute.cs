namespace xunitv3.template.Examples.Ordering;

/// <summary>Lower values run first; methods without the attribute run at <see cref="PriorityOrderer.DefaultPriority"/>.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority => priority;
}
