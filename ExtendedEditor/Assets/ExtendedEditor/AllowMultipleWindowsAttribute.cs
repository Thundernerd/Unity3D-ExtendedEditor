#if UNITY_EDITOR
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AllowMultipleWindowsAttribute : Attribute {

	public AllowMultipleWindowsAttribute() { }
}
#endif