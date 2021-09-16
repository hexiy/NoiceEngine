using System;
using System.Drawing.Design;

namespace Engine
{
	[ShowInEditor]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ShowInEditor : Attribute
    {
        public ShowInEditor()
        {
        }
    }
}
