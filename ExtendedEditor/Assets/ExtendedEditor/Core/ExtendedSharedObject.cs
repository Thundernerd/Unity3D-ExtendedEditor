#if UNITY_EDITOR

namespace TNRD.Editor.Core {
	/// <summary>
	/// Base for shared objects
	/// </summary>
	public class ExtendedSharedObject {

		public ExtendedSharedObject() { }

		public virtual void Update( bool hasFocus ) { }
	}
}
#endif