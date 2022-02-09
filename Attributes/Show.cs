namespace Engine;

[Show]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class Show : Attribute
{
	public Show()
	{
	}
}
