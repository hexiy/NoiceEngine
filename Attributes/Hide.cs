using System;
using System.Drawing.Design;

namespace Engine
{
	[Hide]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class Hide : Attribute
	{
		public Hide()
		{
		}
	}
}
