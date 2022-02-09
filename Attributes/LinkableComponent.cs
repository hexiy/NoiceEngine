namespace Engine;

/// <summary>
/// TARGET MUST BE PUBLIC
/// </summary>
[LinkableComponent]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class LinkableComponent : Attribute
{
}
