using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine
{
	public class FieldOrPropertyInfo
	{
		private FieldInfo fieldInfo;
		private PropertyInfo propertyInfo;
		public bool canShowInEditor = false;
		public FieldOrPropertyInfo (FieldInfo fi)
		{
			fieldInfo = fi;
			UpdateCanShowInEditor ();
		}
		public FieldOrPropertyInfo (PropertyInfo pi)
		{
			propertyInfo = pi;
			UpdateCanShowInEditor ();
		}
		private void UpdateCanShowInEditor ()
		{
			for (int i = 0; i < CustomAttributes.Count (); i++)
			{
				if (CustomAttributes.ElementAtOrDefault (i).AttributeType == typeof (ShowInEditor))
				{
					canShowInEditor = true;
				}
			}
		}
		public object? GetValue (object? obj)
		{
			if (fieldInfo != null)
			{
				return fieldInfo.GetValue (obj);
			}
			if (propertyInfo != null)
			{
				return propertyInfo.GetValue (obj);
			}
			return null;
		}
		public void SetValue (object? obj, object? value)
		{
			if (fieldInfo != null)
			{
				fieldInfo.SetValue (obj, value);
			}
			if (propertyInfo != null)
			{
				if (propertyInfo.GetSetMethod () != null) propertyInfo.SetValue (obj, value);
			}
		}
		public IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				if (fieldInfo != null)
				{
					return fieldInfo.CustomAttributes;
				}
				if (propertyInfo != null)
				{
					return propertyInfo.CustomAttributes;
				}
				return null;
			}
		}
		public string Name
		{
			get
			{
				if (fieldInfo != null)
				{
					return fieldInfo.Name;
				}
				if (propertyInfo != null)
				{
					return propertyInfo.Name;
				}
				return null;
			}
		}
		public Type FieldOrPropertyType
		{
			get
			{
				if (fieldInfo != null)
				{
					return fieldInfo.FieldType;
				}
				if (propertyInfo != null)
				{
					return propertyInfo.PropertyType;
				}
				return null;
			}
		}
	}
}